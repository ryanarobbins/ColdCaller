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
            var Students = db.Students.ToList();
            var ChosenStudents =
                from s in Students
                where s.StudentClass == "1st Period" && s.TeacherId == "DemoTeacher"
                select s;
            

            return View(ChosenStudents);
        }

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
        
        [HttpGet]
        public ActionResult JsonChosenStudents(string ChosenClass)
        {
            var userId = User.Identity.GetUserId();
            var Students = db.Students.ToList();
            var ChosenStudents =
                from s in Students
                where s.StudentClass == ChosenClass && s.TeacherId == userId
                select new {
                    name = s.Name
                };
            var JsonStudents = Json(ChosenStudents, JsonRequestBehavior.AllowGet);
            return JsonStudents;
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult JsonChosenStudentsDemo()
        {
            var userId = "DemoTeacher";
            var Students = db.Students.ToList();
            var ChosenStudents =
                from s in Students
                where s.StudentClass == "1st Period" && s.TeacherId == userId
                select new
                {
                    name = s.Name
                };
            var JsonStudents = Json(ChosenStudents, JsonRequestBehavior.AllowGet);
            return JsonStudents;
        }
    }
}