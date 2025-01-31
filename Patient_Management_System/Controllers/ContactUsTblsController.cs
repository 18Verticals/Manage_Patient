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
    public class ContactUsTblsController : Controller
    {
        private Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();

        // GET: ContactUsTbls
        public ActionResult Index()
        {
            return View(db.ContactUsTbls.ToList());
        }

        // GET: ContactUsTbls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsTbl contactUsTbl = db.ContactUsTbls.Find(id);
            if (contactUsTbl == null)
            {
                return HttpNotFound();
            }
            return View(contactUsTbl);
        }

        // GET: ContactUsTbls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactUsTbls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Feedback_Id,Name,Email,Message,Phone")] ContactUsTbl contactUsTbl)
        {
            if (ModelState.IsValid)
            {
                db.ContactUsTbls.Add(contactUsTbl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactUsTbl);
        }

        // GET: ContactUsTbls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsTbl contactUsTbl = db.ContactUsTbls.Find(id);
            if (contactUsTbl == null)
            {
                return HttpNotFound();
            }
            return View(contactUsTbl);
        }

        // POST: ContactUsTbls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Feedback_Id,Name,Email,Message,Phone")] ContactUsTbl contactUsTbl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactUsTbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactUsTbl);
        }

        // GET: ContactUsTbls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsTbl contactUsTbl = db.ContactUsTbls.Find(id);
            if (contactUsTbl == null)
            {
                return HttpNotFound();
            }
            return View(contactUsTbl);
        }

        // POST: ContactUsTbls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContactUsTbl contactUsTbl = db.ContactUsTbls.Find(id);
            db.ContactUsTbls.Remove(contactUsTbl);
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
