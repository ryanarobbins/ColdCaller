using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ColdCaller.Models;
using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using Microsoft.AspNet.Identity;
using System.IO;
using CsvHelper.Configuration;



namespace ColdCaller.Controllers
{
    
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: Students
        //Lets users input students into the database
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        //POST: Students/AddFromForm
        //Pulls the "Student.Name"s and "Student.StudentClass" 
        //from the text area and text box.
        //Creates new Students and adds them to the database.
        //Creates a list of the new Students and sends it to the view.
        //The view lists the new Students.
        [HttpPost]
        public ActionResult AddFromForm(string StudentClass, string StudentList)
        {
            ViewBag.StudentClass = StudentClass;
            //ViewBag.StudentList = StudentList;
            var TeacherId = User.Identity.GetUserId();

            //Creates an array of strings that holds the names that were entered in the form.
            var parsedString = StudentList.Split(',').Select(sValue => sValue.Trim()).ToArray();

            //int x = parsedString.Length;

            //Creates a new student for each name that was entered into the form and adds it to the database.
            foreach (string s in parsedString)
            {
                var newStudent = new Student();
                newStudent.Name = s;
                newStudent.StudentClass = StudentClass;
                newStudent.TeacherId = TeacherId;
                db.Students.Add(newStudent);
            }

            db.SaveChanges();
            
            return RedirectToAction("List");
        }

        //POST: Students/AddFromCSV
        //Pulls the "Student.Name"s and "Student.StudentClass"es from the CSV doc.
        //Creates new Students and adds them to the database.
        //Creates a list of the new Students and sends it to the view.
        //The view lists the new Students.
        [HttpPost]
        public ActionResult AddFromCSV(HttpPostedFileBase file)
        {
            //###############Need to check to makes sure the file is a CSV#################
            
            if (file != null && file.ContentLength > 0)
                try
                {
                    //Creates a reader for the incoming file.
                    TextReader reader = new StreamReader(file.InputStream);

                    //Using functions from CSVHelper package.
                    //http://joshclose.github.io/CsvHelper/
                    var parsedCSV = new CsvReader(reader);

                    //Configuration settings for CSVHelper.
                    //MyClassMap is a class in StudentsController.cs.
                    parsedCSV.Configuration.RegisterClassMap<MyClassMap>();
                    parsedCSV.Configuration.TrimFields = true;
                    parsedCSV.Configuration.HasHeaderRecord = false;

                    //Uses the map to create Student objects with values from the CSV file.
                    var AddedStudents = parsedCSV.GetRecords<Student>();

                    var TeacherId = User.Identity.GetUserId();

                    //Sets the TeacherId for the new Students and adds them to the database.
                    foreach (var s in AddedStudents)
                    {
                        s.TeacherId = TeacherId;
                        db.Students.Add(s);
                    }

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    return View("Index");
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
                return View("Index");
            }

            ViewBag.Message = "Success!";
                        
            return RedirectToAction("List");
        }


        // GET: Students/List
        // Need to implement paging.
        public ActionResult List()
        {
            var TeacherId = User.Identity.GetUserId();
            return View(db.Students.Where(s => s.TeacherId == TeacherId).ToList().OrderBy(s => s.StudentClass));
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentId,Name,StudentClass")] Student student)
        {
            //The TeacherID is set to the UserId.
            if (ModelState.IsValid)
            {
                student.TeacherId = User.Identity.GetUserId();
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("List");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,StudentClass")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("List");
        }

        // GET: Student/DeleteClass
        // Lets user choose a class to delete
        [HttpGet]
        public ActionResult DeleteClass()
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

        // POST: Student/DeleteClass
        // Deletes the selected class
        [HttpPost]
        public  ActionResult DeleteClass(string StudentClass)
        {
            var TeacherId = User.Identity.GetUserId();
            
            db.Students.RemoveRange(db.Students.Where(s => s.StudentClass == StudentClass && s.TeacherId == TeacherId));
            db.SaveChanges();
            
            return RedirectToAction("List");
        }

        //For use by CSVHelper functions.
        //Specifies that the first column in the CSV maps
        //to the same and the second column to the StudentClass
        //of a student object.
        public sealed class MyClassMap : CsvClassMap<Student>
        {
            public MyClassMap()
            {
                Map(m => m.Name).Index(0);
                Map(m => m.StudentClass).Index(1);
                
            }
        }

        //I am probably not disposing of things properly and
        //need work on that.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
