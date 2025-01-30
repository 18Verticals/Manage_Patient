using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Patient_Management_System.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Test()
        {
            return View();
        }
        public ActionResult Doctors()
        {
            return View();
        }
        public ActionResult home_healthcare()
        {
            return View();
        }
        public ActionResult Livedoc_swasthya_1()
        {
            return View();
        }

        public ActionResult Livedoc_swasthya_2()
        {
            return View();
        }

        public ActionResult Livedoc_swasthya_3()
        {
            return View();
        }

        public ActionResult Livedoc_Niramaya_1()
        {
            return View();
        }

        public ActionResult Livedoc_Niramaya_2()
        {
            return View();
        }

        public ActionResult Livedoc_Niramaya_3()
        {
            return View();
        }
        public ActionResult Organ_Donation()
        {
            return View();
        }

        public ActionResult lung_transplant()
        {
            return View();
        }

        public ActionResult heart_transplant()
        {
            return View();
        }

        public ActionResult renal_transplantt()
        {
            return View();
        }

        public ActionResult liver_transplant()
        {
            return View();
        }

        public ActionResult cornea_transplant()
        {
            return View();
        }

        public ActionResult Aboutus()
        {
            return View();
        }

        public ActionResult contactUs()
        {
            return View();
        }

        public ActionResult second_opinion()
        {
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult Demo()
        {
            return View();
        }

        
    }
}