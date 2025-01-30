using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Patient_Management_System.Models;

namespace Patient_Management_System.Controllers
{
    public class AppointmentTblsController : Controller
    {
        private Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();

        // GET: AppointmentTbls
        public ActionResult Index()
        {
            var appointmentTbls = db.AppointmentTbls.Include(a => a.DepartmentTbl).Include(a => a.DoctorTbl).Include(a => a.PatientsTbl).Include(a => a.ScheduleTbl);
            return View(appointmentTbls.ToList());
        }

        // GET: AppointmentTbls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentTbl appointmentTbl = db.AppointmentTbls.Find(id);
            if (appointmentTbl == null)
            {
                return HttpNotFound();
            }
            return View(appointmentTbl);
        }

        // GET: AppointmentTbls/Create
        public ActionResult Create()
        {
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName");
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_Id", "P_FirstName");
            ViewBag.Schedule_ID = new SelectList(db.ScheduleTbls, "Schedule_ID", "Available_Date");
            ViewBag.TimeSlots = GetTimeSlots();
            return View();
        }
        private List<SelectListItem> GetTimeSlots()
        {
            List<SelectListItem> timeSlots = new List<SelectListItem>();
            TimeSpan startTime = new TimeSpan(9, 30, 0); // 9:30 AM
            TimeSpan endTime = new TimeSpan(18, 30, 0); // 11:30 PM

            while (startTime <= endTime)
            {
                string timeValue = startTime.ToString(@"hh\:mm"); // Format: 08:00
                timeSlots.Add(new SelectListItem { Value = timeValue, Text = timeValue });
                startTime = startTime.Add(new TimeSpan(0, 30, 0)); // Increment by 30 minutes
            }

            return timeSlots;
        }
        // POST: AppointmentTbls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Appointment_ID,Doctor_ID,Patient_ID,Dept_ID,Schedule_ID,Apt_Date,Apt_Time,Description")] AppointmentTbl appointmentTbl)
        {
            if (ModelState.IsValid)
            {
                db.AppointmentTbls.Add(appointmentTbl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", appointmentTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", appointmentTbl.Doctor_ID);
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_Id", "P_FirstName", appointmentTbl.Patient_ID);
            ViewBag.Schedule_ID = new SelectList(db.ScheduleTbls, "Schedule_ID", "Available_Date", appointmentTbl.Schedule_ID);
            return View(appointmentTbl);
        }






        // GET: AppointmentTbls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentTbl appointmentTbl = db.AppointmentTbls.Find(id);
            if (appointmentTbl == null)
            {
                return HttpNotFound();
            }
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", appointmentTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", appointmentTbl.Doctor_ID);
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_Id", "P_FirstName", appointmentTbl.Patient_ID);
            ViewBag.Schedule_ID = new SelectList(db.ScheduleTbls, "Schedule_ID", "Available_Date", appointmentTbl.Schedule_ID);
            return View(appointmentTbl);
        }

        // POST: AppointmentTbls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Appointment_ID,Doctor_ID,Patient_ID,Dept_ID,Schedule_ID,Apt_Date,Apt_Time,Description")] AppointmentTbl appointmentTbl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointmentTbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", appointmentTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", appointmentTbl.Doctor_ID);
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_Id", "P_FirstName", appointmentTbl.Patient_ID);
            ViewBag.Schedule_ID = new SelectList(db.ScheduleTbls, "Schedule_ID", "Available_Date", appointmentTbl.Schedule_ID);
            return View(appointmentTbl);
        }

        // GET: AppointmentTbls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentTbl appointmentTbl = db.AppointmentTbls.Find(id);
            if (appointmentTbl == null)
            {
                return HttpNotFound();
            }
            return View(appointmentTbl);
        }

        // POST: AppointmentTbls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppointmentTbl appointmentTbl = db.AppointmentTbls.Find(id);
            db.AppointmentTbls.Remove(appointmentTbl);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
