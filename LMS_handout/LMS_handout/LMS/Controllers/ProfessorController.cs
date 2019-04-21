﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Students(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Class(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Categories(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      return View();
    }

    public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      ViewData["uid"] = uid;
      return View();
    }

    /*******Begin code to modify********/


    /// <summary>
    /// Returns a JSON array of all the students in a class.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "dob" - date of birth
    /// "grade" - the student's grade in this class
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
    {
            //Listing is SubjectAbbreviation
            var query = from classes in db.Classes
                        join course in db.Courses on classes.CId equals course.CId
                        join enrolledClass in db.Enrolled on classes.ClassId equals enrolledClass.ClassId
                        join student in db.Students on enrolledClass.UId equals student.UId
                        where course.Listing == subject &&
                        course.Number == num &&
                        classes.Semester.Equals(season+year)
                        select new
                        {
                            fname = student.FName,
                            lname = student.LName,
                            uid = student.UId,
                            dob = new DateTime((long)student.Dob),
                            grade = enrolledClass.Grade 
                        };
      return Json(query.ToArray());
    }



    /// <summary>
    /// Returns a JSON array with all the assignments in an assignment category for a class.
    /// If the "category" parameter is null, return all assignments in the class.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The assignment category name.
    /// "due" - The due DateTime
    /// "submissions" - The number of submissions to the assignment
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class, 
    /// or null to return assignments from all categories</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
    {
            //Listing is Subject Abbreviation
            if (category!=null)
            {
                var query = from courses in db.Courses 
                            join classes in db.Classes on courses.CId equals classes.CId
                            join assignmentCat in db.AssignmentCategories on classes.ClassId equals assignmentCat.ClassId
                            join assignment in db.Assignments on assignmentCat.AcId equals assignment.AcId
                            join submission in db.Submission on assignment.AId equals submission.AId
                            where courses.Listing == subject &&
                            courses.Number == num &&
                            classes.Semester.Equals(season + year) &&
                            assignmentCat.Name.Equals(category) &&
                            assignment.AId == submission.AId
                            select new
                            {
                                aname = assignment.Name,
                                cname = assignmentCat.Name,
                                due = assignment.Due,
                                submissions = (from count in db.Submission group count by count.AId).Distinct().Count()
                            };
                return Json(query.ToArray());
            }
            var anotherQuery = from courses in db.Courses
                        join classes in db.Classes on courses.CId equals classes.CId
                        join assignmentCat in db.AssignmentCategories on classes.ClassId equals assignmentCat.ClassId
                        join assignment in db.Assignments on assignmentCat.AcId equals assignment.AcId
                        join submission in db.Submission on assignment.AId equals submission.AId
                        where courses.Listing == subject &&
                        courses.Number == num &&
                        classes.Semester.Equals(season + year) &&
                        assignment.AId == submission.AId
                        select new
                        {
                            aname = assignment.Name,
                            cname = assignmentCat.Name,
                            due = assignment.Due,
                            submissions = (from count in db.Submission group count by count.AId).Distinct().Count()
                        };
            return Json(anotherQuery.ToArray());
        }


    /// <summary>
    /// Returns a JSON array of the assignment categories for a certain class.
    /// Each object in the array should have the folling fields:
    /// "name" - The category name
    /// "weight" - The category weight
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
    {
            var query = from courses in db.Courses
                        join classes in db.Classes on courses.CId equals classes.CId
                        join assignmentCat in db.AssignmentCategories on classes.ClassId equals assignmentCat.ClassId
                        where courses.Listing == subject &&
                        courses.Number == num &&
                        classes.Semester.Contains(season + year)
                        select new
                        {
                            name = assignmentCat.Name,
                            weight = assignmentCat.GradeWeight
                        };
      return Json(query.ToArray());
    }

    /// <summary>
    /// Creates a new assignment category for the specified class.
    /// If a category of the given class with the given name already exists, return success = false.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The new category name</param>
    /// <param name="catweight">The new category weight</param>
    /// <returns>A JSON object containing {success = true/false} </returns>
    public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
    {
            //Check if the Assignment Category exists
            var query = from courses in db.Courses
                        join classes in db.Classes on courses.CId equals classes.CId
                        join assignmentCat in db.AssignmentCategories on classes.ClassId equals assignmentCat.ClassId
                        where courses.Listing == subject &&
                        courses.Number == num &&
                        classes.Semester.Contains(season + year) &&
                        assignmentCat.Name == category
                        select new
                        {
                            temp = assignmentCat.ClassId
                        };
            if (query.Count() == 0)
            {
                var anotherQuery = from courses in db.Courses
                            join classes in db.Classes on courses.CId equals classes.CId
                            where courses.Listing == subject &&
                            courses.Number == num &&
                            classes.Semester.Contains(season + year)
                            select new
                            {
                                classes.ClassId
                            };
                uint classID = anotherQuery.First().ClassId;
                AssignmentCategories newAssignCat = new AssignmentCategories
                {
                    Name = category,
                    ClassId = classID,
                    GradeWeight = (uint)catweight
                };
                db.AssignmentCategories.Add(newAssignCat);
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
    }

    /// <summary>
    /// Creates a new assignment for the given class and category.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="asgpoints">The max point value for the new assignment</param>
    /// <param name="asgdue">The due DateTime for the new assignment</param>
    /// <param name="asgcontents">The contents of the new assignment</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
    {
            var createAssign = from assignCat in db.AssignmentCategories
                               join classes in db.Classes on assignCat.ClassId equals classes.ClassId
                               join course in db.Courses on classes.CId equals course.CId
                               where course.Listing == subject &&
                               course.Number == num &&
                               classes.Semester.Equals(season + year) &&
                               assignCat.Name == category
                               select new
                               {
                                   assignCat.AcId
                               };
            uint aClassID = createAssign.First().AcId;
            Assignments newAssignment = new Assignments
            {
                AcId = aClassID,
                Name = asgname,
                Contents = asgcontents,
                Due = asgdue,
                Points = (uint)asgpoints
            };
            //Change all student scores
            var query = (from submissions in db.Submission
                        join students in db.Students on submissions.UId equals students.UId
                        join assign in db.Assignments on submissions.AId equals assign.AId
                        join assignCat in db.AssignmentCategories on assign.AcId equals assignCat.AcId
                        join classes in db.Classes on assignCat.ClassId equals classes.ClassId
                        join course in db.Courses on classes.CId equals course.CId
                        where course.Listing == subject &&
                               course.Number == num &&
                               classes.Semester.Equals(season + year)
                        select new
                        {
                            currentStudent = students.UId,
                            studentScore = from submissions in db.Submission
                                           join targetStudents in db.Students on submissions.UId equals students.UId
                                           join assign in db.Assignments on submissions.AId equals assign.AId
                                           join assignCat in db.AssignmentCategories on assign.AcId equals assignCat.AcId
                                           join classes in db.Classes on assignCat.ClassId equals classes.ClassId
                                           join course in db.Courses on classes.CId equals course.CId
                                           where course.Listing == subject &&
                                                  course.Number == num &&
                                                  targetStudents.UId == students.UId &&
                                                  classes.Semester.Equals(season + year)
                                           select new
                                           {
                                               score = submissions.Score
                                           },
                     totalScore = from submissions in db.Submission
                                  join targetStudents in db.Students on submissions.UId equals students.UId
                                  join assign in db.Assignments on submissions.AId equals assign.AId
                                  join assignCat in db.AssignmentCategories on assign.AcId equals assignCat.AcId
                                  join classes in db.Classes on assignCat.ClassId equals classes.ClassId
                                  join course in db.Courses on classes.CId equals course.CId
                                  where course.Listing == subject &&
                                         course.Number == num &&
                                         targetStudents.UId == students.UId &&
                                         classes.Semester.Equals(season + year)
                                  select new
                                  {
                                      points = assign.Points
                                  },
                     grade = from enrolled in db.Enrolled where students.UId == enrolled.UId && 
                             classes.ClassId == enrolled.ClassId
                             select enrolled.Grade
                        }).ToList();
            uint totalPoints = 0;
            foreach (var student in query)
            {
                string currentUID = student.currentStudent;
                uint currentScore = 0;
                if (totalPoints == 0)
                {
                    if (student.totalScore.ToList().Count !=0)
                    {
                        foreach (var point in student.totalScore)
                        {
                            totalPoints += point.points;
                        }
                    }
                    
                }
                if(student.studentScore.ToList().Count != 0)
                {
                    foreach (var score in student.studentScore)
                    {
                        currentScore += score.score;
                    }
                }
                double percentGrade = currentScore / totalPoints * 100;
                //HOW TO REWRITE ROW!!!!!!!!!!!!!!1
                //student.studentScore = currentScore;

            }
      return Json(new { success = false });
    }


    /// <summary>
    /// Gets a JSON array of all the submissions to a certain assignment.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "time" - DateTime of the submission
    /// "score" - The score given to the submission
    /// 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
    {
            var query = from submissions in db.Submission
                        join students in db.Students on submissions.UId equals students.UId
                        join assign in db.Assignments on submissions.AId equals assign.AId
                        join assignCat in db.AssignmentCategories on assign.AcId equals assignCat.AcId
                        join classes in db.Classes on assignCat.ClassId equals classes.ClassId
                        join course in db.Courses on classes.CId equals course.CId
                        where course.Listing == subject &&
                               course.Number == num &&
                               assignCat.Name == category &&
                               classes.Semester.Equals(season + year) &&
                               assign.Name == asgname
                        select new
                        {
                            fname = students.FName,
                            lname = students.LName,
                            uid = students.UId,
                            time = submissions.Time,
                            score =submissions.Score
                        };
                               
      return Json(query.ToList());
    }


    /// <summary>
    /// Set the score of an assignment submission
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <param name="uid">The uid of the student who's submission is being graded</param>
    /// <param name="score">The new score for the submission</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
    {
            var query = from submissions in db.Submission
                        join students in db.Students on submissions.UId equals students.UId
                        join assign in db.Assignments on submissions.AId equals assign.AId
                        join assignCat in db.AssignmentCategories on assign.AcId equals assignCat.AcId
                        join classes in db.Classes on assignCat.ClassId equals classes.ClassId
                        join course in db.Courses on classes.CId equals course.CId
                        where course.Listing == subject &&
                               course.Number == num &&
                               assignCat.Name == category &&
                               classes.Semester.Equals(season + year) &&
                               assign.Name == asgname &&
                               students.UId == "u" + uid
                        select new
                        {

                        };
            //update current students grade
      return Json(new { success = true });
    }


    /// <summary>
    /// Returns a JSON array of the classes taught by the specified professor
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester in which the class is taught
    /// "year" - The year part of the semester in which the class is taught
    /// </summary>
    /// <param name="uid">The professor's uid</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
            var query = from user in db.User
                        join professor in db.Professors on user.UId equals professor.UId
                        join aClass in db.Classes on user.UId equals aClass.Teacher
                        join courses in db.Courses on aClass.CId equals courses.CId
                        where aClass.Teacher == "u" + uid
                        select new
                        {
                            subject = courses.Listing,
                            number =  courses.Number,
                            name = courses.Name,
                            season = aClass.Semester.Substring(0, aClass.Semester.Length-4),
                            year = aClass.Semester.Substring(aClass.Semester.Length - 4)
                        };
      return Json(query.ToArray());
    }


    /*******End code to modify********/

  }
}