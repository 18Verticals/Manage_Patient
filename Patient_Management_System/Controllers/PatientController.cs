using Patient_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patient_Management_System.Models;
using System.Configuration;

namespace Patient_Management_System.Controllers
{
    public class PatientController : Controller
    {
        private readonly Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;

        // GET: Patient
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Appointment()
        {
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName");
           
            ViewBag.TimeSlots = GetTimeSlots();
            return View();
        }

        [HttpGet]
        public ActionResult Contact_Us()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact_Us(ContactVM contact)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            if (ModelState.IsValid)
            {

                try
                {
                    using (conn)
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_Add_Contact", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Name", contact.Name);
                            cmd.Parameters.AddWithValue("@Email", contact.Email);
                            cmd.Parameters.AddWithValue("@Message", contact.Message);
                            cmd.Parameters.AddWithValue("@Phone", contact.Phone);
                     
                        }
                    }
                    return RedirectToAction("Index", "Admin");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "An error occurred: " + ex.Message;
                    System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }
            return View(contact);
        }


        private List<SelectListItem> GetTimeSlots()
        {
            List<SelectListItem> timeSlots = new List<SelectListItem>();
            TimeSpan startTime = new TimeSpan(9, 30, 0); 
            TimeSpan endTime = new TimeSpan(18, 30, 0); 

            while (startTime <= endTime)
            {
                string timeValue = startTime.ToString(@"hh\:mm");
                timeSlots.Add(new SelectListItem { Value = timeValue, Text = timeValue });
                startTime = startTime.Add(new TimeSpan(0, 30, 0));
            }
            return timeSlots;
        }

        [HttpPost]
        public ActionResult Appointment(AppointmentVM aptVM)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            if (ModelState.IsValid)
            {
                try
                {
                    using (conn)
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_Add_Appointment", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Change Patient_ID to P_FirstName to match stored procedure logic
                            cmd.Parameters.AddWithValue("@P_FirstName", aptVM.P_FirstName);
                            cmd.Parameters.AddWithValue("@Doctor_ID", aptVM.Doctor_ID);
                            cmd.Parameters.AddWithValue("@Dept_ID", aptVM.Dept_ID);
                            cmd.Parameters.AddWithValue("@Apt_Date", aptVM.Apt_Date);
                            cmd.Parameters.AddWithValue("@Apt_Time", aptVM.Apt_Time);
                            cmd.Parameters.AddWithValue("@Description", aptVM.Description);
                            cmd.Parameters.AddWithValue("@Phone", aptVM.Phone);
                            cmd.Parameters.AddWithValue("@Diseases", aptVM.Diseases);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    TempData["SuccessMessage"] = "Appointment successfully created.";
                    return RedirectToAction("Index", "Admin");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "An error occurred: " + ex.Message;
                    System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
                }
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }

            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", aptVM.Doctor_ID);
            ViewBag.TimeSlots = GetTimeSlots();

            return View(aptVM);
        }
    }
}