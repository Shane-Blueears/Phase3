using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
  public class CommonController : Controller
  {

    /*******Begin code to modify********/

    // TODO: Uncomment and change 'X' after you have scaffoled

    
    protected Team69LMSContext db;

    public CommonController()
    {
      db = new Team69LMSContext();
    }
    

    /*
     * WARNING: This is the quick and easy way to make the controller
     *          use a different LibraryContext - good enough for our purposes.
     *          The "right" way is through Dependency Injection via the constructor 
     *          (look this up if interested).
    */

    // TODO: Uncomment and change 'X' after you have scaffoled
    
    public void UseLMSContext(Team69LMSContext ctx)
    {
      db = ctx;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
    



    /// <summary>
    /// Retreive a JSON array of all departments from the database.
    /// Each object in the array should have a field called "name" and "subject",
    /// where "name" is the department name and "subject" is the subject abbreviation.
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetDepartments()
    {
            // TODO: Do not return this hard-coded array.
            var query = from department in db.Departments
                        select new
                        {
                            name = department.Name,
                            subject = department.SubA

                        };
            return Json(query.ToArray());
        }



    /// <summary>
    /// Returns a JSON array representing the course catalog.
    /// Each object in the array should have the following fields:
    /// "subject": The subject abbreviation, (e.g. "CS")
    /// "dname": The department name, as in "Computer Science"
    /// "courses": An array of JSON objects representing the courses in the department.
    ///            Each field in this inner-array should have the following fields:
    ///            "number": The course number (e.g. 5530)
    ///            "cname": The course name (e.g. "Database Systems")
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetCatalog()
    {
            //Listing is Sub Abbreviation
            var query = from department in db.Departments
                        select new
                        {
                            dname = department.Name,
                            subject = department.SubA,
                            courses = from course in db.Courses where department.SubA == course.Listing
                                      select new { number = course.Number, cname = course.Name }
                        };
            return Json(query.ToArray());
    }

    /// <summary>
    /// Returns a JSON array of all class offerings of a specific course.
    /// Each object in the array should have the following fields:
    /// "season": the season part of the semester, such as "Fall"
    /// "year": the year part of the semester
    /// "location": the location of the class
    /// "start": the start time in format "hh:mm:ss"
    /// "end": the end time in format "hh:mm:ss"
    /// "fname": the first name of the professor
    /// "lname": the last name of the professor
    /// </summary>
    /// <param name="subject">The subject abbreviation, as in "CS"</param>
    /// <param name="number">The course number, as in 5530</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetClassOfferings(string subject, int number)
    {
            //Listing is SubjectAbbreviation
            var query = from classes in db.Classes
                        join course in db.Courses on classes.CId equals course.CId
                        join departments in db.Departments on course.Listing equals departments.SubA
                        join professor in db.Professors on classes.Teacher equals professor.UId
                        where departments.SubA == subject &&
                        course.Number == number
                        select new
                        {
                            season = classes.Semester.Substring(0, classes.Semester.Length-4),
                            year = classes.Semester.Substring(classes.Semester.Length-4),
                            location = classes.Loc,
                            start = classes.STime,
                            end = classes.ETime,
                            fname = professor.FName,
                            lname = professor.LName
                        };
            return Json(query.ToArray());
        }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <returns>The assignment contents</returns>
    public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
    {
            //Listing is SubjectAbbreviation
            var query = from classes in db.Classes
                        join course in db.Courses on classes.CId equals course.CId
                        join departments in db.Departments on course.Listing equals departments.SubA
                        join assignmentCategories in db.AssignmentCategories on classes.ClassId equals assignmentCategories.ClassId
                        join assignment in db.Assignments on assignmentCategories.AcId equals assignment.AcId
                        where departments.SubA == subject &&
                        course.Number == num &&
                        classes.Semester.Contains(season+year) &&
                        assignmentCategories.Name == category &&
                        assignment.Name ==asgname
                        select new
                        {
                            assignment.Contents
                        };

            return Content(query.ToString());
    }


    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment submission.
    /// Returns the empty string ("") if there is no submission.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <param name="uid">The uid of the student who submitted it</param>
    /// <returns>The submission text</returns>
    public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
    {
            //Listing is SubjectAbbreviation
            var query = from classes in db.Classes
                        join course in db.Courses on classes.CId equals course.CId
                        join departments in db.Departments on course.Listing equals departments.SubA
                        join assignmentCategories in db.AssignmentCategories on classes.ClassId equals assignmentCategories.ClassId
                        join assignment in db.Assignments on assignmentCategories.AcId equals assignment.AcId
                        join submission in db.Submission on assignment.AId equals submission.AId
                        where departments.SubA == subject &&
                        course.Number == num &&
                        classes.Semester.Contains(season + year) &&
                        assignmentCategories.Name == category &&
                        assignment.Name == asgname &&
                        submission.UId == "u"+uid
                        select new
                        {
                            submission.Contents
                        };
            if(query.ToString().Length ==0)
            {
                return Content("");
            }
            return Content(query.ToString());
        }


    /// <summary>
    /// Gets information about a user as a single JSON object.
    /// The object should have the following fields:
    /// "fname": the user's first name
    /// "lname": the user's last name
    /// "uid": the user's uid
    /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
    ///               If the user is a Professor, this is the department they work in.
    ///               If the user is a Student, this is the department they major in.    
    ///               If the user is an Administrator, this field is not present in the returned JSON
    /// </summary>
    /// <param name="uid">The ID of the user</param>
    /// <returns>
    /// The user JSON object 
    /// or an object containing {success: false} if the user doesn't exist
    /// </returns>
    public IActionResult GetUser(string uid)
    {
            var studentQuery = from user in db.User
                        join student in db.Students on user.UId equals student.UId
                        join departments in db.Departments on student.Major equals departments.SubA
                        where student.UId == "u"+uid
                        select new
                        {
                            fname = student.FName,
                            lname = student.LName,
                            uid = student.UId,
                            department = departments.Name
                        };
            var professorQuery = from user in db.User
                                 join professor in db.Professors on user.UId equals professor.UId
                                 join departments in db.Departments on professor.WorksIn equals departments.SubA
                                 where professor.UId == "u" + uid
                                 select new
                                 {
                                     fname = professor.FName,
                                     lname = professor.LName,
                                     uid = professor.UId,
                                     department = departments.Name
                                 };
            var administratorQuery = from user in db.User
                                     join admin in db.Administrators on user.UId equals admin.UId
                                     where admin.UId == "u" + uid
                                     select new
                                     {
                                         fname = admin.FName,
                                         lname = admin.LName,
                                         uid = admin.UId,
                                     };

            if (studentQuery.ToArray().Length != 0)
            {
                return Json(studentQuery.Single());
            }
            else if(professorQuery.ToArray().Length != 0)
            {
                return Json(professorQuery.Single());
            }
            else if(administratorQuery.ToArray().Length != 0)
            {
                return Json(administratorQuery.Single());
            }
            else
            {
                return Json(new { success = false });
            } 
    }


    /*******End code to modify********/

  }
}