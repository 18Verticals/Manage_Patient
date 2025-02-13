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

        [HttpPost]
        public ActionResult Appointment(AppointmentVM aptVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", aptVM.Doctor_ID);
                ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
                return View(aptVM);
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[BookAppointment]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Pass required parameters
                    cmd.Parameters.AddWithValue("@Doctor_ID", aptVM.Doctor_ID);
                    cmd.Parameters.AddWithValue("@Dept_ID", aptVM.Dept_ID);
                    cmd.Parameters.AddWithValue("@Apt_Date", aptVM.Apt_Date);
                    cmd.Parameters.AddWithValue("@Apt_Time", aptVM.Apt_Time);
                    cmd.Parameters.AddWithValue("@Description", aptVM.Description);
                    cmd.Parameters.AddWithValue("@Phone", aptVM.Phone);
                    cmd.Parameters.AddWithValue("@Diseases", aptVM.Diseases);

                    // Capture the return value
                    SqlParameter returnValue = new SqlParameter
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(returnValue);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    int result = (returnValue.Value != DBNull.Value) ? Convert.ToInt32(returnValue.Value) : -2;

                    if (result == 1)
                    {
                        ViewBag.Message = "Appointment booked successfully!";
                    }
                    else if (result == 0)
                    {
                        ViewBag.Message = "This time slot is already booked!";
                    }
                    else if (result == -1)
                    {
                        ViewBag.Message = "No patient exists with this phone number.";
                    }
                    else
                    {
                        ViewBag.Message = "An unexpected error occurred.";
                    }
                }
            }

            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", aptVM.Doctor_ID);
            ViewBag.TimeSlots = GetTimeSlots();
            return View(aptVM);
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

        [HttpGet]
        public JsonResult GetAvailableSlots(int doctorId, DateTime date)
        {
            List<string> availableSlots = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                SELECT DISTINCT FORMAT(TimeSlot, 'hh\:mm tt') AS TimeSlot FROM TimeSlots 
                WHERE TimeSlot NOT IN (
                    SELECT Apt_Time FROM AppointmentTbl 
                    WHERE Doctor_ID = @Doctor_ID AND Apt_Date = @Apt_Date
                )";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);
                    cmd.Parameters.AddWithValue("@Apt_Date", date);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            availableSlots.Add(dr["TimeSlot"].ToString());
                        }
                    }
                }
            }
            return Json(availableSlots, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search_Doctor(string searchTerm)
        {
            List<DoctorVM> doctors = new List<DoctorVM>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SearchDoctors", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", searchTerm ?? string.Empty);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            doctors.Add(new DoctorVM
                            {
                                Doctor_ID = Convert.ToInt32(dr["Doctor_ID"]),
                                Dr_FirstName = dr["Dr_FirstName"].ToString(),
                                Dept_Name = dr["Dept_Name"].ToString(),
                                Dr_Qualification = dr["Dr_Qualification"].ToString(),
                                Dr_ImagePath = dr["Dr_ImagePath"].ToString(),
                                Fees = Convert.ToInt32(dr["Fees"]),
                            });
                        }
                    }
                }
            }
            if (doctors.Count == 0)
            {
                ViewBag.Message = "No doctors found.";
            }
            return View(doctors);
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


       
    }
}