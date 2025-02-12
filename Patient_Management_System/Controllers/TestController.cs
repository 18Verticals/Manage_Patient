using Patient_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Patient_Management_System.Models;

namespace Patient_Management_System.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        private readonly Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
        
        
        
        [HttpGet]
        public ActionResult BookAppointment()
        {
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName");
            return View();
        }
            [HttpPost]
        public ActionResult BookAppointment(AppointmentVM model)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("BookAppointment", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", model.Doctor_ID);
                    cmd.Parameters.AddWithValue("@Patient_ID", model.Patient_ID);
                    cmd.Parameters.AddWithValue("@Apt_Date", model.Apt_Date);
                    cmd.Parameters.AddWithValue("@Apt_Time", model.Apt_Time);

                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        ViewBag.Message = "Appointment booked successfully!";
                    }
                    catch (SqlException)
                    {
                        ViewBag.Message = "This time slot is already booked!";
                    }
                }
            }
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", model.Doctor_ID);

            return View();
        }
        public JsonResult GetAvailableSlots(int doctorId, DateTime date)
        {
            List<TimeSpan> availableSlots = new List<TimeSpan>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
            SELECT DISTINCT TimeSlot FROM TimeSlots 
            WHERE TimeSlot NOT IN (
                SELECT Apt_Tme FROM AppointmentTbl 
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
                            availableSlots.Add((TimeSpan)dr["TimeSlot"]);
                        }
                    }
                }
            }

            return Json(availableSlots, JsonRequestBehavior.AllowGet);
        }

    }

}