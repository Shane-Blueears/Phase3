using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : CommonController
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
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


        public IActionResult ClassListings(string subject, string num)
        {
            System.Diagnostics.Debug.WriteLine(subject + num);
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            uid = "u" + uid;
            var classQuery = (from student in db.Students
                              join enrolled in db.Enrolled on student.UId equals enrolled.UId
                              join classes in db.Classes on enrolled.ClassId equals classes.ClassId
                              join courses in db.Courses on classes.CId equals courses.CId
                              where uid == student.UId
                              select new
                              {
                                  // course listing is subA
                                  subject = courses.Listing,
                                  number = courses.Number,
                                  name = courses.Name,
                                  season = classes.Semester.Substring(0, classes.Semester.Length - 4),
                                  year = classes.Semester.Substring(classes.Semester.Length - 4),
                                  grade = enrolled.Grade
                              }

                      ).ToList();
            return Json(classQuery);
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
        {
            string semester = season + year;
            uid = "u" + uid;
            var assignQuery = (from students in db.Students
                               join enrolled in db.Enrolled on students.UId equals enrolled.UId
                               join classes in db.Classes on enrolled.ClassId equals classes.ClassId
                               join courses in db.Courses on classes.CId equals courses.CId
                               join assignCats in db.AssignmentCategories on classes.ClassId equals assignCats.ClassId
                               join assignments in db.Assignments on assignCats.AcId equals assignments.AcId
                               where subject == courses.Listing && num == courses.Number && semester == classes.Semester && uid == students.UId
                               select new
                               {
                                   aname = assignments.Name,
                                   cname = assignCats.Name,
                                   due = assignments.Due,
                                   score = assignments.Points
                               }
                ).ToList();
            return Json(assignQuery);
        }



        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// The score of the submission should start as 0 until a Professor grades it
        /// If a Student submits to an assignment again, it should replace the submission contents
        /// and the submission time (the score should remain the same).
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
          string category, string asgname, string uid, string contents)
        {
            string semester = season + year;
            uid = "u" + uid;
            var assignQuery = (from students in db.Students
                               join enrolled in db.Enrolled on students.UId equals enrolled.UId
                               join classes in db.Classes on enrolled.ClassId equals classes.ClassId
                               join courses in db.Courses on classes.CId equals courses.CId
                               join assignCats in db.AssignmentCategories on classes.ClassId equals assignCats.ClassId
                               join assignments in db.Assignments on assignCats.AcId equals assignments.AcId
                               where subject == courses.Listing && num == courses.Number && semester == classes.Semester
                               && category == assignCats.Name && asgname == assignments.Name && uid == students.UId
                               select new
                               {
                                   AId = assignments.AId,
                                   AcId = assignments.AcId,
                                   Name = assignments.Name,
                                   Content = assignments.Contents,
                               }
                );

            if (assignQuery.Count() == 0)
                return Json(new { success = false });

            var subCountQuery = (from submission in db.Submission
                                 orderby submission.SId
                                 select new { submission.SId }
                                 );

            var subQuery = (from submission in db.Submission
                            where submission.UId == uid
                            select new
                            {
                                Contents = submission.Contents
                            }).ToList();

            if (subQuery.Count == 0)
            {
                Submission newSubmission = new Submission
                {
                   //SId = subCountQuery.First().SId + 1,
                    AId = assignQuery.First().AId,
                    UId = uid,
                    Contents = contents,
                    Score = 0,
                    Time = DateTime.Now
                };
                db.Submission.Add(newSubmission);
                db.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                Submission prev = (from submission in db.Submission
                                   where submission.UId == uid
                                   select submission).First();
                prev.Contents = contents;
                db.SaveChanges();
                return Json(new { success = true });
            }



        }


        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. 
        /// false if the student is already enrolled in the class, true otherwise.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            string semester = season + year;
            uid = "u" + uid;
            var stuEnrolled = (from students in db.Students
                               join enrolled in db.Enrolled on students.UId equals enrolled.UId
                               join classes in db.Classes on enrolled.ClassId equals classes.ClassId
                               join courses in db.Courses on classes.CId equals courses.CId
                               where uid == students.UId && subject == courses.Listing && num == courses.Number && semester == classes.Semester
                               select new
                               {
                                   students.UId
                               }
                         );

            if (stuEnrolled.Count() == 0)
            {
                // Make sure the class exists
                var classQuery = (from classes in db.Classes
                                  join courses in db.Courses on classes.CId equals courses.CId
                                  where subject == courses.Listing && num == courses.Number && semester == classes.Semester
                                  select new
                                  {
                                      classes.ClassId
                                  }
                                  );
                if (classQuery.Count() != 0)
                {
                    Enrolled newEnroll = new Enrolled
                    {
                        UId = "u"+uid,
                        ClassId = classQuery.First().ClassId,
                        Grade = ""
                    };
                    db.Enrolled.Add(newEnroll);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }



        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// Assume all classes are 4 credit hours.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// If a student is not enrolled in any classes, they have a GPA of 0.0.
        /// Otherwise, the point-value of a letter grade is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            double sum = 0;
            uid = "u" + uid;
            var gradeQuery = (from students in db.Students
                              join enrolled in db.Enrolled on students.UId equals enrolled.UId
                              where uid == students.UId
                              select enrolled.Grade);
            int entries = gradeQuery.Count();
            foreach (var grade in gradeQuery)
            {
                switch (grade)
                {
                    case "A":
                        sum += 4.0;
                        break;
                    case "A-":
                        sum += 3.7;
                        break;
                    case "B+":
                        sum += 3.3;
                        break;
                    case "B":
                        sum += 3.0;
                        break;
                    case "B-":
                        sum += 2.7;
                        break;
                    case "C+":
                        sum += 2.3;
                        break;
                    case "C":
                        sum += 2.0;
                        break;
                    case "C-":
                        sum += 1.7;
                        break;
                    case "D+":
                        sum += 1.3;
                        break;
                    case "D":
                        sum += 1.0;
                        break;
                    case "D-":
                        sum += 0.7;
                        break;
                    case "E":
                        sum += 0.0;
                        break;
                    default:
                        entries--;
                        break;
                }
            }


            return Json(new { GPA = (sum / entries) });
        }

        /*******End code to modify********/

    }
}