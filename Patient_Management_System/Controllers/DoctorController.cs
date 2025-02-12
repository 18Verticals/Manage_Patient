using Patient_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patient_Management_System.ViewModel;

namespace Patient_Management_System.Controllers
{
    public class DoctorController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;

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
        public ActionResult Login(string email, string password)
        {
            DoctorVM doctor = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Dr_Login", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Dr_Email", email);
                    cmd.Parameters.AddWithValue("@Dr_Password", password);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        doctor = new DoctorVM
                        {
                            Doctor_ID = Convert.ToInt32(reader["Doctor_ID"]),
                            Dr_FirstName = reader["Dr_FirstName"].ToString(),
                            Dr_Email = reader["Dr_Email"].ToString()
                        };
                    }

                    reader.Close();
                }
            }

            if (doctor != null)
            {
                Session["Doctor_ID"] = doctor.Doctor_ID;
                Session["Dr_FirstName"] = doctor.Dr_FirstName;
                return RedirectToAction("Appointments");
            }

            ViewBag.Error = "Invalid email or password!";
            return View();
        }

        // GET: Doctor/Appointments
        public ActionResult Appointments()
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");

            int doctorId = Convert.ToInt32(Session["Doctor_ID"]);
            List<AppointmentVM> appointments = new List<AppointmentVM>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDrAppointments", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        appointments.Add(new AppointmentVM
                        {
                            Appointment_ID = reader["Appointment_ID"] != DBNull.Value ? Convert.ToInt32(reader["Appointment_ID"]) : 0,
                            Patient_ID = reader["Patient_ID"] != DBNull.Value ? Convert.ToInt32(reader["Patient_ID"]) : 0,
                            Apt_Date = reader["Apt_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Apt_Date"]) : DateTime.MinValue,
                            Email = reader["Email"] as string ?? string.Empty,
                            Diseases = reader["Diseases"] as string ?? string.Empty,
                            Apt_Time = reader["Apt_Time"] != DBNull.Value ? (TimeSpan?)reader["Apt_Time"] : null,
                            Description = reader["Description"] as string ?? string.Empty,




                        });
                    }

                    reader.Close();
                }
            }

            return View(appointments);
        }

        // GET: Doctor/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    

        public ActionResult Test()
        {
            return View();
        }
    }
}
  


