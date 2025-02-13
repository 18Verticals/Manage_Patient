using Patient_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Patient_Management_System.Controllers
{
    public class Dr_LoginController : Controller
    {
        // GET: Dr_Login
        public ActionResult Dr_Login()
        {
            return View();
        }

    }
}