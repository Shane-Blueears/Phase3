﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Administrator")]
  public class AdministratorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Department(string subject)
    {
      ViewData["subject"] = subject;
      return View();
    }

    public IActionResult Course(string subject, string num)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }

    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of all the courses in the given department.
    /// Each object in the array should have the following fields:
    /// "number" - The course number (as in 5530)
    /// "name" - The course name (as in "Database Systems")
    /// </summary>
    /// <param name="subject">The department subject abbreviation (as in "CS")</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetCourses(string subject)
    {
            // Listing is subA
            var query =(
                from department in db.Departments
                join course in db.Courses on department.SubA equals course.Listing
                where course.Listing == subject
                select new
                {
                    number = course.Number,
                    name = course.Name
                }).ToList();


            return Json(query);
    }


    


    /// <summary>
    /// Returns a JSON array of all the professors working in a given department.
    /// Each object in the array should have the following fields:
    /// "lname" - The professor's last name
    /// "fname" - The professor's first name
    /// "uid" - The professor's uid
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetProfessors(string subject)
    {
            var query = (
                from department in db.Departments
                join prof in db.Professors on department.SubA equals prof.WorksIn
                where department.SubA == subject
                select new
                {
                    lname = prof.LName,
                    fname = prof.FName,
                    uid = prof.UId
                }).ToList();
      return Json(query);
    }



    /// <summary>
    /// Creates a course.
    /// A course is uniquely identified by its number + the subject to which it belongs
    /// </summary>
    /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
    /// <param name="number">The course number</param>
    /// <param name="name">The course name</param>
    /// <returns>A JSON object containing {success = true/false}.
    /// false if the course already exists, true otherwise.</returns>
    public IActionResult CreateCourse(string subject, int number, string name)
    {
            var hasCourse = (from course in db.Courses
                             where course.Number == number && course.Listing == subject
                             select course).FirstOrDefault() != null;
                             

            if (!hasCourse)
            {
                var createQuery = from course in db.Courses
                                  orderby course.CId
                                  select new
                                  {
                                      course.CId
                                  };
                Random rnd = new Random();
                uint newCid = createQuery.First().CId + (uint)rnd.Next(100);
                Courses newCouse = new Courses
                {
                    CId = newCid,
                    Name = name,
                    Listing = subject,
                    Number = (uint)number
                };

                db.Courses.Add(newCouse);
                db.SaveChanges();
                return Json(new { success = true });

            }
            

               

      return Json(new { success = false });
    }



    /// <summary>
    /// Creates a class offering of a given course.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="number">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="start">The start time</param>
    /// <param name="end">The end time</param>
    /// <param name="location">The location</param>
    /// <param name="instructor">The uid of the professor</param>
    /// <returns>A JSON object containing {success = true/false}. 
    /// false if another class occupies the same location during any time 
    /// within the start-end range in the same semester, or if there is already
    /// a Class offering of the same Course in the same Semester,
    /// true otherwise.</returns>
    public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
    {
            bool locAvailable = false;
            var locQuery = (from loc in db.Classes
                            where loc.Loc == location
                            select loc);

            foreach(var loc in locQuery)
            {
                if ((start.CompareTo(loc.STime) < 0) && (end.CompareTo(loc.STime) < 0) ||
                                (start.CompareTo(loc.ETime) > 0 && end.CompareTo(loc.ETime) > 0))
                {
                    locAvailable = true;
                }
            }

            string semester = season + year;
            var hasExisted = (from classes in db.Classes
                              where semester == classes.Semester
                              select classes
                              ).FirstOrDefault() != null;

            if (locAvailable && !hasExisted)
            {

                
            }

      return Json(new { success = false });
    }


    /*******End code to modify********/

  }
}