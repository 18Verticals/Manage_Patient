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
    public class DoctorController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(DoctorTbl doctor)
        {
            if (ModelState.IsValid)
            {
                string str = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
                SqlConnection conn = new SqlConnection(str);
                try
                {
                    using (conn)
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("[sp_Dr_LoginInfo]", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Dr_Email", doctor.Dr_Email);
                            cmd.Parameters.AddWithValue("@Dr_Password", doctor.Dr_Password);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {

                                return RedirectToAction("Index", "Admin");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid email or password.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "An error occurred: " + ex.Message;
                    System.Diagnostics.Debug.WriteLine("Login error: " + ex.Message);
                }
            }
            return View(doctor);
        }
    }
}