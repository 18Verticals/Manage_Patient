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
    public class ScheduleTblsController : Controller
    {
        private Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();

        // GET: ScheduleTbls
        public ActionResult Index()
        {
            var scheduleTbls = db.ScheduleTbls.Include(s => s.DepartmentTbl).Include(s => s.DoctorTbl);
            return View(scheduleTbls.ToList());
        }

        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleTbl scheduleTbl = db.ScheduleTbls.Find(id);
            if (scheduleTbl == null)
            {
                return HttpNotFound();
            }
            return View(scheduleTbl);
        }

      
        public ActionResult Create()
        {
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
            ViewBag.Doctor_ID = new SelectList(
      db.DoctorTbls.Select(d => new {
          Doctor_ID = d.Doctor_ID,
          FullName = d.Dr_FirstName + " " + d.Dr_LastName}),"Doctor_ID","FullName"); return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Schedule_ID,Doctor_ID,Dept_ID,Available_Days,Start_Time,End_Time,Status")] ScheduleTbl scheduleTbl)
        {
            if (ModelState.IsValid)
            {
                db.ScheduleTbls.Add(scheduleTbl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", scheduleTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(
       db.DoctorTbls.Select(d => new {
           Doctor_ID = d.Doctor_ID,
           FullName = d.Dr_FirstName + " " + d.Dr_LastName }), "Doctor_ID", "FullName",scheduleTbl.Doctor_ID
   ); return View(scheduleTbl);
        }

       
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleTbl scheduleTbl = db.ScheduleTbls.Find(id);
            if (scheduleTbl == null)
            {
                return HttpNotFound();
            }
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", scheduleTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", scheduleTbl.Doctor_ID);
            return View(scheduleTbl);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Schedule_ID,Doctor_ID,Dept_ID,Available_Days,Start_Time,End_Time,Status")] ScheduleTbl scheduleTbl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduleTbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", scheduleTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", scheduleTbl.Doctor_ID);
            return View(scheduleTbl);
        }

       
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduleTbl scheduleTbl = db.ScheduleTbls.Find(id);
            if (scheduleTbl == null)
            {
                return HttpNotFound();
            }
            return View(scheduleTbl);
        }

        // POST: ScheduleTbls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ScheduleTbl scheduleTbl = db.ScheduleTbls.Find(id);
            db.ScheduleTbls.Remove(scheduleTbl);
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
