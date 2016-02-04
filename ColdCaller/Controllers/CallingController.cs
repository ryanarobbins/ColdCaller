using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ColdCaller.Models;
using System.Data;
using System.Data.Entity;
using System.Net;
using Microsoft.AspNet.Identity;


namespace ColdCaller.Controllers
{
    
    public class CallingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Calling
        //Page where user selects which class they want to cold call
        [HttpGet]
        public ActionResult Index()
        {
            //create a list of distinct values from the StudentClass field
            //of the Students table
            var userId = User.Identity.GetUserId();
            IEnumerable<SelectListItem> studentClasses = db.Students
              .Where(c => c.TeacherId == userId)
              .Select(c => new SelectListItem
              {
                  Value = c.StudentClass,
                  Text = c.StudentClass
              }).Distinct();

            //put the list of StudentClasses in the ViewBag so we can use
            //them for the drop down list in the view
            ViewBag.StudentClass = studentClasses;

            return View();
        }

        //POST: Calling
        //
        [HttpPost]
        public ActionResult Index(string StudentClass)
        {

            IEnumerable<Student> ChosenStudents = FilterStudents(StudentClass);

            ViewBag.ChosenClass = StudentClass;
            return View("VerifyClass", ChosenStudents);
        }
        
        public ActionResult ColdCall(string ChosenClass)
        {
            ViewBag.ChosenClass = ChosenClass;
            IEnumerable<Student> ChosenStudents = FilterStudents(ChosenClass);
            
            
            return View(ChosenStudents);
        }
        
        [AllowAnonymous]
        public ActionResult ColdCallDemo()
        {

            ViewBag.ChosenClass = "DemoTeacher";
            /*var Students = db.Students.ToList();
            var ChosenStudents =
                from s in Students
                where s.StudentClass == "1st Period" && s.TeacherId == "DemoTeacher"
                select s;
            */

            return View();
        }

        //Returns a list of students that match the UserId and chosen class.
        private IEnumerable<Student> FilterStudents(string StudentClass) 
        {
            var userId = User.Identity.GetUserId();
            var Students = db.Students.ToList();
            var ChosenStudents =
                from s in Students
                where s.StudentClass == StudentClass  && s.TeacherId == userId
                select s;
            return ChosenStudents;
        
        }
        
        // GET: Calling/JsonChosenStudents
        // Creates a collection of the Names of the Student objects
        // that are in the ChosenClass for the current user.
        // Converts them to JSON and sends the JSON to the browser.
        [HttpGet]
        public ActionResult JsonChosenStudents(string ChosenClass)
        {
            var userId = User.Identity.GetUserId();

            var Students = db.Students.ToList();
            
            // Goes through the list of Students in the database and
            // creates a collection of the Names from Students who
            // match the StudentClass and TeacherId.
            var ChosenStudents =
                from s in Students
                where s.StudentClass == ChosenClass && s.TeacherId == userId
                select new {
                    name = s.Name
                };

            // Converts the collection of names into a JsonResult and returns it.
            // The list of names will be sent to the browser as a JSON string.
            var JsonStudents = Json(ChosenStudents, JsonRequestBehavior.AllowGet);
            return JsonStudents;
        }

        // Esentially a duplicate of JsonChosenStudents that will
        // be accessible without logging in.  It pulls records with
        // TeacherId of DemoTeacher and StudentClass of 1st Period 
        // that were added to the database.
        [HttpGet]
        [AllowAnonymous]
        public ActionResult JsonChosenStudentsDemo()
        {
            // Setting up demo values
            var userId = "DemoTeacher";
            var ChosenClass = "1st Period";

            var Students = db.Students.ToList();
            
            // This is the same code that is in JsonChosenStudents.
            // Should be encapsulated into a reusable method.
            //################################
            var ChosenStudents =
                from s in Students
                where s.StudentClass == ChosenClass && s.TeacherId == userId
                select new
                {
                    name = s.Name
                };
            var JsonStudents = Json(ChosenStudents, JsonRequestBehavior.AllowGet);
            return JsonStudents;
            //################################
        }
    }
}