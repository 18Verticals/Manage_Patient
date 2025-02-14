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
using static System.Collections.Specialized.BitVector32;

using System.Data.Entity;
using System.Net;


namespace Patient_Management_System.Controllers
{
    public class DoctorController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
        private readonly Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();


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
                            Phone = reader["Phone"] as string ?? string.Empty,
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

        public ActionResult Add_Schedule()
        {
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
            ViewBag.Doctor_ID = new SelectList(
          db.DoctorTbls.Select(d => new {
              Doctor_ID = d.Doctor_ID,
              FullName = d.Dr_FirstName + " " + d.Dr_LastName
          }), "Doctor_ID", "FullName"); return View();
        }

        [HttpPost]
        public ActionResult Add_Schedule([Bind(Include = "Schedule_ID,Doctor_ID,Dept_ID,Available_Date,Start_Time,End_Time,Status")] ScheduleTbl scheduleTbl)
        {
            if (ModelState.IsValid)
            {
                db.ScheduleTbls.Add(scheduleTbl);
                db.SaveChanges();
                return RedirectToAction("List_Schedule");
            }

            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", scheduleTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(
            db.DoctorTbls.Select(d => new {
                Doctor_ID = d.Doctor_ID,
                FullName = d.Dr_FirstName + " " + d.Dr_LastName
            }), "Doctor_ID", "FullName", scheduleTbl.Doctor_ID);
            return View(scheduleTbl);
        }


        public ActionResult List_Schedule()
        {
            var scheduleTbls = db.ScheduleTbls.Include(s => s.DepartmentTbl).Include(s => s.DoctorTbl);
            return View(scheduleTbls.ToList());
        }


        public ActionResult Edit_Schedule(int? id)
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
        public ActionResult Edit_Schedule([Bind(Include = "Schedule_ID,Doctor_ID,Dept_ID,Available_Date,Start_Time,End_Time,Status")] ScheduleTbl scheduleTbl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheduleTbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("List_Schedule");
            }
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", scheduleTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", scheduleTbl.Doctor_ID);
            return View(scheduleTbl);
        }




        [HttpGet]
        public ActionResult Delete_Schedule(int id)
        {
            var schedule = db.ScheduleTbls.Find(id);
            if (schedule == null)
            {
                return HttpNotFound();
            }
            return View(schedule);
        }

        [HttpPost, ActionName("Delete_Schedule")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var schedule = db.ScheduleTbls.Find(id);
            if (schedule != null)
            {
                db.ScheduleTbls.Remove(schedule);
                db.SaveChanges();
            }
            return RedirectToAction("List_Schedule");
        }


        public ActionResult Delete_Prescription(int PrescId)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                using (conn)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Delete_Prescription", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Presc_ID", PrescId);
                        cmd.ExecuteNonQuery();
                    }
                }
                // Correct controller name in Redirect
                return RedirectToAction("Prescription", "Doctor");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while deleting the prescription: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);

                // Instead of redirecting to Admin Payment List, show the Prescription list again
                return RedirectToAction("Prescription", "Doctor");
            }
        }
      
        // GET: Doctor/AddPrescription
        public ActionResult Add_Prescription()
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_Id", "P_FirstName");
            return View();
        }

        // POST: Doctor/AddPrescription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add_Prescription(PrescriptionVM model)
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("[sp_AddDrPrescription]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Doctor_ID", Convert.ToInt32(Session["Doctor_ID"]));
                        cmd.Parameters.AddWithValue("@Patient_ID", model.Patient_ID);
                        cmd.Parameters.AddWithValue("@Medication", model.Medication);
                        cmd.Parameters.AddWithValue("@Dosage", model.Dosage);
                        cmd.Parameters.AddWithValue("@Instructions", model.Instructions);
                        

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("Prescription"); // Redirect to prescription list after adding
            }
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_ID", "P_FirstName", model.Patient_ID);
            return View(model);
        }



        // GET: Doctor/Prescription
        public ActionResult Prescription()
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");

            int doctorId = Convert.ToInt32(Session["Doctor_ID"]);
            List<PrescriptionVM> prescriptions = new List<PrescriptionVM>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDrPrescription", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        prescriptions.Add(new PrescriptionVM
                        {
                            Presc_ID = reader["Presc_ID"] != DBNull.Value ? Convert.ToInt32(reader["Presc_ID"]) : 0,
                            Patient_ID = reader["Patient_ID"] != DBNull.Value ? Convert.ToInt32(reader["Patient_ID"]) : 0,
                            DateIssued = reader["DateIssued"] != DBNull.Value ? Convert.ToDateTime(reader["DateIssued"]) : DateTime.MinValue,
                            Medication = reader["Medication"] as string ?? string.Empty,
                            Dosage = reader["Dosage"] as string ?? string.Empty,
                            Instructions = reader["Instructions"] as string ?? string.Empty,
                        });
                    }
                    reader.Close();
                }
            }

            return View(prescriptions);
        }
    }
}
    
