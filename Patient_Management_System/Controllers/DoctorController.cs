using Patient_Management_System.Models;
using PagedList;
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
using System.IO;
using System.Web.Security;
using System.Net.Mail;


namespace Patient_Management_System.Controllers
{
    public class DoctorController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
        private readonly Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();



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
                using (SqlCommand cmd = new SqlCommand("sp_Dr_LoginInfo", con))
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

        private SelectList GetDoctors()
        {
            List<SelectListItem> doctors = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Doctor_ID, Dr_FirstName + ' ' + ISNULL(Dr_LastName, '') AS FullName FROM DoctorTbl", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            doctors.Add(new SelectListItem
                            {
                                Value = reader["Doctor_ID"].ToString(),
                                Text = reader["FullName"].ToString()
                            });
                        }
                    }
                }
            }

            return new SelectList(doctors, "Value", "Text");
        }

        private SelectList GetDepartment()
        {
            List<SelectListItem> department = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Dept_ID, Dept_Name FROM DepartmentTbl", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            department.Add(new SelectListItem
                            {
                                Value = reader["Dept_ID"].ToString(),
                                Text = reader["Dept_Name"].ToString()
                            });
                        }
                    }
                }
            }
            return new SelectList(department, "Value", "Text");
        }

        private SelectList GetPatients()
        {
            List<SelectListItem> patients = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Patient_Id, P_FirstName + ' ' + ISNULL(P_MiddleName + ' ', '') + ISNULL(P_LastName, '') AS FullName FROM PatientsTbl", conn))

                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            patients.Add(new SelectListItem
                            {
                                Value = reader["Patient_ID"].ToString(),
                                Text = reader["FullName"].ToString()
                            });
                        }
                    }
                }
            }

            return new SelectList(patients, "Value", "Text");
        }
        public ActionResult Profile()
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");

            int doctorId = Convert.ToInt32(Session["Doctor_ID"]);
            List<DoctorVM> doctorList = new List<DoctorVM>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Dr_Profile", conn);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DoctorVM doctor = new DoctorVM
                    {
                        Doctor_ID = reader["Doctor_ID"] != DBNull.Value ? Convert.ToInt32(reader["Doctor_ID"]) : 0,
                        Dr_FirstName = reader["Dr_FirstName"] != DBNull.Value ? reader["Dr_FirstName"].ToString() : "",
                        Dr_LastName = reader["Dr_LastName"] != DBNull.Value ? reader["Dr_LastName"].ToString() : "",
                        Dept_ID = reader["Dept_ID"] != DBNull.Value ? Convert.ToInt32(reader["Dept_ID"]) : 0,
                        Dept_Name = reader["Dept_Name"] != DBNull.Value ? reader["Dept_Name"].ToString() : "",
                        Dr_Email = reader["Dr_Email"] != DBNull.Value ? reader["Dr_Email"].ToString() : "",
                        Dr_Password = reader["Dr_Password"] != DBNull.Value ? reader["Dr_Password"].ToString() : "",
                        Dr_DOB = reader["Dr_DOB"] != DBNull.Value ? Convert.ToDateTime(reader["Dr_DOB"]) : DateTime.MinValue,
                        Dr_Gender = reader["Dr_Gender"] != DBNull.Value ? reader["Dr_Gender"].ToString() : "",
                        Dr_Phone = reader["Dr_Phone"] != DBNull.Value ? reader["Dr_Phone"].ToString() : "",
                        Dr_Qualification = reader["Dr_Qualification"] != DBNull.Value ? reader["Dr_Qualification"].ToString() : "",
                        Dr_Address = reader["Dr_Address"] != DBNull.Value ? reader["Dr_Address"].ToString() : "",
                        Dr_City = reader["Dr_City"] != DBNull.Value ? reader["Dr_City"].ToString() : "",
                        Dr_State = reader["Dr_State"] != DBNull.Value ? reader["Dr_State"].ToString() : "",
                        Dr_Pincode = reader["Dr_Pincode"] != DBNull.Value ? Convert.ToInt32(reader["Dr_Pincode"]) : 0,
                        Dr_ImagePath = reader["Dr_ImagePath"] != DBNull.Value ? reader["Dr_ImagePath"].ToString() : "",
                        Fees = reader["Fees"] != DBNull.Value ? Convert.ToInt32(reader["Fees"]) : 0,
                        Dr_Status = reader["Dr_Status"] != DBNull.Value ? reader["Dr_Status"].ToString() : "",
                    };
                    doctorList.Add(doctor);
                }
            }
            return View(doctorList);
        }



        [HttpGet]
        public ActionResult Edit_Prescription(int PrescId)
        {
            var presc = db.PrescriptionTbls.FirstOrDefault(d => d.Presc_ID == PrescId);
            if (presc == null)
            {
                return HttpNotFound();
            }

            PrescriptionVM prescVM = new PrescriptionVM
            {
                Presc_ID = presc.Presc_ID,
                Patient_Name = db.PatientsTbls.Where(p => p.Patient_Id == presc.Patient_ID).Select(p => p.P_FirstName).FirstOrDefault(),
                Doctor_Name = db.DoctorTbls.Where(d => d.Doctor_ID == presc.Doctor_ID).Select(d => d.Dr_FirstName).FirstOrDefault(),
                Medication = presc.Medication,
                Instructions = presc.Instructions,
                Dosage = presc.Dosage,
                DateIssued = presc.DateIssued
            };

            ViewBag.Patient_Name = new SelectList(GetPatients(), "Text", "Text", prescVM.Patient_Name);
            ViewBag.Doctor_Name = new SelectList(GetDoctors(), "Text", "Text", prescVM.Doctor_Name);

            return View(prescVM);
        }




        [HttpPost]
        public ActionResult Edit_Prescription(PrescriptionVM prescVM)
        {
            try
            {
                ViewBag.Patient_ID = new SelectList(GetPatients(), "Value", "Text", prescVM.Patient_ID);
                ViewBag.Doctor_ID = new SelectList(GetDoctors(), "Value", "Text", prescVM.Doctor_ID);

                if (!int.TryParse(prescVM.Doctor_Name, out int doctorId) ||
                    !int.TryParse(prescVM.Patient_Name, out int patientId))
                {
                    TempData["Error"] = "Invalid doctor or Patient selection.";
                    return View(prescVM);
                }

                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_Edit_Prescription", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Presc_ID", prescVM.Presc_ID);
                            cmd.Parameters.AddWithValue("@Patient_ID", patientId);
                            cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);
                            cmd.Parameters.AddWithValue("@Medication", prescVM.Medication);
                            cmd.Parameters.AddWithValue("@Instructions", prescVM.Instructions);
                            cmd.Parameters.AddWithValue("@Dosage", prescVM.Dosage);
                            cmd.Parameters.AddWithValue("@DateIssued", prescVM.DateIssued);
                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    TempData["SuccessMessage"] = "Prescription record have been updated successfully.";
                    return RedirectToAction("Prescription");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
            }


            return View(prescVM);
        }


        [HttpGet]
        public ActionResult Edit_Profile(int doctorId)
        {
            var doctor = db.DoctorTbls.Where(d => d.Doctor_ID == doctorId).FirstOrDefault();
            if (doctor == null)
            {
                TempData["Error"] = "Doctor not found.";
                return RedirectToAction("#");
            }
            DoctorVM doctorVM = new DoctorVM
            {
                Doctor_ID = doctor.Doctor_ID,
                Dr_FirstName = doctor.Dr_FirstName,
                Dr_LastName = doctor.Dr_LastName,
                Dr_Email = doctor.Dr_Email,
                Dr_DOB = doctor.Dr_DOB,
                Dr_Gender = doctor.Dr_Gender,
                Dr_Phone = doctor.Dr_Phone,
                Dr_Qualification = doctor.Dr_Qualification,
                Dr_Address = doctor.Dr_Address,
                Dr_City = doctor.Dr_City,
                Dr_State = doctor.Dr_State,
                Dr_Pincode = doctor.Dr_Pincode.HasValue ? (int)doctor.Dr_Pincode.Value : 0,
                Dr_ImagePath = doctor.Dr_ImagePath,
                Dr_Status = doctor.Dr_Status,
                Fees = doctor.Fees.HasValue ? (int)doctor.Fees.Value : 0,
                Dept_ID = doctor.Dept_ID.HasValue ? (int)doctor.Dept_ID.Value : 0,
            };
            ViewBag.Dept_ID = GetDepartment();
            return View(doctorVM);
        }



        [HttpPost]
        public ActionResult Edit_Profile(DoctorVM doctor, HttpPostedFileBase Dr_ImagePath)
        {
            try
            {
                if (Dr_ImagePath != null && Dr_ImagePath.ContentLength > 0)
                {
                    string uploadPath = Server.MapPath("~/Content/UploadedImages/");
                    string fileName = Path.GetFileNameWithoutExtension(Dr_ImagePath.FileName);
                    string extension = Path.GetExtension(Dr_ImagePath.FileName);
                    string newImagePath = "/Content/UploadedImages/" + fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    try
                    {
                        Dr_ImagePath.SaveAs(Server.MapPath(newImagePath));
                        doctor.Dr_ImagePath = newImagePath;

                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Error saving image: " + ex.Message;
                        return View(doctor);
                    }
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Edit_Doctor", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Doctor_ID", doctor.Doctor_ID);
                        cmd.Parameters.AddWithValue("@Dr_FirstName", doctor.Dr_FirstName);
                        cmd.Parameters.AddWithValue("@Dr_LastName", doctor.Dr_LastName);
                        cmd.Parameters.AddWithValue("@Dr_Email", doctor.Dr_Email);
                        cmd.Parameters.AddWithValue("@Dr_DOB", (object)doctor.Dr_DOB ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dr_Gender", doctor.Dr_Gender ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dr_Phone", doctor.Dr_Phone ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dr_Qualification", doctor.Dr_Qualification);
                        cmd.Parameters.AddWithValue("@Dr_Address", doctor.Dr_Address ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dr_City", doctor.Dr_City ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dr_State", doctor.Dr_State ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Dr_Pincode", doctor.Dr_Pincode);
                        cmd.Parameters.AddWithValue("@Dr_ImagePath", doctor.Dr_ImagePath);


                        cmd.Parameters.AddWithValue("@Dr_Status", doctor.Dr_Status ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Fees", doctor.Fees);
                        cmd.Parameters.AddWithValue("@Dept_ID", doctor.Dept_ID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Message"] = "Doctor record updated successfully.";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(doctor);
            }
        }

        public ActionResult Appointments(DateTime? selectedDate)
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
                    cmd.Parameters.AddWithValue("@Selected_Date", selectedDate.HasValue ? selectedDate.Value.Date : (object)DBNull.Value);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        appointments.Add(new AppointmentVM
                        {
                            Appointment_ID = reader["Appointment_ID"] != DBNull.Value ? Convert.ToInt32(reader["Appointment_ID"]) : 0,
                            Patient_ID = reader["Patient_ID"] != DBNull.Value ? Convert.ToInt32(reader["Patient_ID"]) : 0,
                            P_FirstName = reader["P_FirstName"] != DBNull.Value ? reader["P_FirstName"].ToString() : string.Empty,
                            Apt_Date = reader["Apt_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Apt_Date"]) : DateTime.MinValue,
                            Phone = reader["Phone"] as string ?? string.Empty,
                            Diseases = reader["Diseases"] as string ?? string.Empty,
                            Apt_Time = reader["Apt_Time"] != DBNull.Value ? (TimeSpan?)reader["Apt_Time"] : null,
                            Description = reader["Description"] as string ?? string.Empty
                        });
                    }

                    reader.Close();
                }
            }

            ViewBag.SelectedDate = selectedDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");
            return View(appointments);
        }


        public ActionResult Today_Appointments()
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
                        DateTime appointmentDate = reader["Apt_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Apt_Date"]) : DateTime.MinValue;

                        if (appointmentDate.Date == DateTime.Today)
                        {
                            appointments.Add(new AppointmentVM
                            {
                                Appointment_ID = reader["Appointment_ID"] != DBNull.Value ? Convert.ToInt32(reader["Appointment_ID"]) : 0,
                                Patient_ID = reader["Patient_ID"] != DBNull.Value ? Convert.ToInt32(reader["Patient_ID"]) : 0,
                                P_FirstName = reader["P_FirstName"] as string ?? string.Empty,
                                Apt_Date = appointmentDate,
                                Phone = reader["Phone"] as string ?? string.Empty,
                                Diseases = reader["Diseases"] as string ?? string.Empty,
                                Apt_Time = reader["Apt_Time"] != DBNull.Value ? (TimeSpan?)reader["Apt_Time"] : null,
                                Description = reader["Description"] as string ?? string.Empty,

                            });
                        }
                    }
                    reader.Close();
                }
            }

            return View(appointments);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


        public ActionResult Add_Schedule()
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]

        public ActionResult Add_Schedule(ScheduleVM model)
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");

            model.Doctor_ID = Convert.ToInt32(Session["Doctor_ID"]);

            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddSchedule", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Doctor_ID", model.Doctor_ID);
                        cmd.Parameters.AddWithValue("@Start_Time", model.Start_Time);
                        cmd.Parameters.AddWithValue("@End_Time", model.End_Time);
                        cmd.Parameters.AddWithValue("@Status", model.Status);
                        cmd.Parameters.AddWithValue("@Available_Date", model.Available_Date);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Schedule record have been Added successfully.";
                return RedirectToAction("List_Schedule");
            }
            return View(model);
        }


        public ActionResult List_Schedule(ScheduleVM scheduleVM, int? page, string searchQuery)
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");

            int doctorId = Convert.ToInt32(Session["Doctor_ID"]);
            List<ScheduleVM> schedules = new List<ScheduleVM>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDrSchedule", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        schedules.Add(new ScheduleVM
                        {
                            Schedule_ID = reader["Schedule_ID"] != DBNull.Value ? Convert.ToInt32(reader["Schedule_ID"]) : 0,
                            Doctor_ID = reader["Doctor_ID"] != DBNull.Value ? Convert.ToInt32(reader["Doctor_ID"]) : 0,
                            Dr_FirstName = reader["Dr_FirstName"] as string ?? "N/A",
                            Dr_LastName = reader["Dr_LastName"] as string ?? "N/A",
                            Dept_ID = reader["Dept_ID"] != DBNull.Value ? Convert.ToInt32(reader["Dept_ID"]) : 0,
                            Dept_Name = reader["Dept_Name"] as string ?? "N/A",
                            Available_Date = reader["Available_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Available_Date"]) : DateTime.MinValue,
                            Status = reader["Status"] as string ?? "N/A",

                            Start_Time = reader.IsDBNull(reader.GetOrdinal("Start_Time"))
                                ? (TimeSpan?)null
                                : (TimeSpan)reader["Start_Time"],

                            End_Time = reader.IsDBNull(reader.GetOrdinal("End_Time"))
                                ? (TimeSpan?)null
                                : (TimeSpan)reader["End_Time"],
                        });
                    }

                    reader.Close();
                }
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                schedules = schedules
                    .Where(s =>
                        (s.Dr_FirstName != null && s.Dr_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (s.Dept_Name != null && s.Dept_Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (s.Available_Date.HasValue && s.Available_Date.Value.ToString("yyyy-MM-dd").Contains(searchQuery))
                    ).ToList();
            }



            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(schedules.ToPagedList(pageNumber, pageSize));
        }


        [HttpGet]
        public ActionResult Edit_Schedule(int scheduleId)
        {
            var schedule = db.ScheduleTbls.Where(d => d.Schedule_ID == scheduleId).FirstOrDefault();
            if (schedule == null)
            {
                TempData["Error"] = "Schedule not found.";
                return RedirectToAction("List_Doctor");
            }

            ScheduleVM scheduleVM = new ScheduleVM
            {
                Schedule_ID = schedule.Schedule_ID,
                Doctor_ID = schedule.Doctor_ID,
                Dept_ID = schedule.Dept_ID,
                Start_Time = schedule.Start_Time,
                End_Time = schedule.End_Time,
                Available_Date = schedule.Available_Date,
                Status = schedule.Status,
            };
            ViewBag.Dept_ID = GetDepartment();
            ViewBag.Doctor_ID = GetDoctors();
            return View(scheduleVM);
        }


        [HttpPost]
        public ActionResult Edit_Schedule(ScheduleVM scheduleVM)
        {
            try
            {
                var existingSchedule = db.ScheduleTbls
                                         .Where(s => s.Schedule_ID == scheduleVM.Schedule_ID)
                                         .Select(s => new { s.Doctor_ID, s.Dept_ID })
                                         .FirstOrDefault();

                if (existingSchedule == null)
                {
                    TempData["Error"] = "Schedule not found.";
                    return RedirectToAction("List_Schedule");
                }

                var affectedPatients = new List<(string Email, string PatientName)>();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Edit_Schedule", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Schedule_ID", scheduleVM.Schedule_ID);
                        cmd.Parameters.AddWithValue("@Doctor_ID", existingSchedule.Doctor_ID);
                        cmd.Parameters.AddWithValue("@Dept_ID", existingSchedule.Dept_ID);
                        cmd.Parameters.AddWithValue("@Start_Time", scheduleVM.Start_Time);
                        cmd.Parameters.AddWithValue("@End_Time", scheduleVM.End_Time);
                        cmd.Parameters.AddWithValue("@Available_Date", scheduleVM.Available_Date);
                        cmd.Parameters.AddWithValue("@Status", scheduleVM.Status);

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                affectedPatients.Add((reader["PatientEmail"].ToString(), reader["PatientName"].ToString()));
                            }
                        }
                    }
                }

                if (affectedPatients.Any())
                {
                    string doctorName = GetDoctorName(existingSchedule.Doctor_ID);
                    string formattedDate = scheduleVM.Available_Date?.ToString("dd-MM-yyyy") ?? "N/A";
                    string formattedTime = DateTime.Today.Add(scheduleVM.Start_Time ?? TimeSpan.Zero).ToString("hh:mm tt");

                    foreach (var patient in affectedPatients)
                    {
                        SendEmailCancellationNotification(patient.Email, patient.PatientName, doctorName, formattedDate, formattedTime);
                    }

                    TempData["SuccessMessage"] = "Schedule updated successfully. Affected patients have been notified.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Schedule updated successfully.";
                }

                return RedirectToAction("List_Schedule");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(scheduleVM);
            }
        }

        private string GetDoctorName(int doctorId)
        {
            using (var db = new Patient_Management_SystemEntities())
            {
                return db.DoctorTbls.Where(d => d.Doctor_ID == doctorId).Select(d => d.Dr_FirstName).FirstOrDefault();
            }
        }
        private void SendEmailCancellationNotification(string email, string patientName, string doctorName, string appointmentDate, string appointmentTime)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("No email found for the patient.");
                    return;
                }

                string emailBody = $@"
                <html>
                <body>
                    <p>Dear {patientName},</p>
                    <p>We regret to inform you that your appointment with <strong>Dr. {doctorName}</strong> on <strong>{appointmentDate}</strong> at <strong>{appointmentTime}</strong> has been <strong>canceled</strong>.</p>
                    <p>If you have any questions or would like to reschedule, please contact us.</p>
                    <p>Thank you for understanding.</p>
                    <p>Best Regards,<br/>LiveDoc Multispecialist Hospital</p>
                    <p>Any Query? Please Contact Us: 70465 90890</p>
                </body>
                </html>";

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("hemangkanzariya00@gmail.com"),
                    Subject = "Appointment Cancellation Notification",
                    Body = emailBody,
                    IsBodyHtml = true
                };
                mail.To.Add(email);
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("hemangkanzariya00@gmail.com", "lqri ukod qdsl qyfx"),
                    EnableSsl = true
                };
                smtp.Send(mail);
                Console.WriteLine("Cancellation email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);

                Console.WriteLine(ex.ToString());
            }
        }


        public ActionResult Delete_Schedule(int scheduleId)
        {
            try
            {
                var affectedPatients = new List<(string Email, string PatientName, string DoctorName, string AvailableDate, string StartTime)>();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Delete_Schedule", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Schedule_ID", scheduleId);

                        con.Open();

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                affectedPatients.Add((
                                    reader["PatientEmail"].ToString(),
                                    reader["PatientName"].ToString(),
                                    reader["DoctorName"].ToString(),
                                    reader["AvailableDate"].ToString(),
                                    reader["StartTime"].ToString()
                                ));
                            }
                        }
                    }
                }

                if (affectedPatients.Any())
                {
                    foreach (var patient in affectedPatients)
                    {
                        try
                        {
                            SendEmailCancellationNotification(
                                patient.Email,
                                patient.PatientName,
                                patient.DoctorName,
                                patient.AvailableDate,
                                patient.StartTime
                            );
                            Console.WriteLine($"Email sent successfully to {patient.Email}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to send email to {patient.Email}: {ex.Message}");
                        }
                    }
                    TempData["SuccessMessage"] = "Schedule deleted successfully. Affected patients have been notified.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Schedule deleted successfully.";
                }

                return RedirectToAction("List_Schedule");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete_Schedule: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the schedule: " + ex.Message;
                return RedirectToAction("List_Schedule");
            }
        }

        public ActionResult Prescription(int? page, string searchQuery)
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
                            P_FirstName = reader["P_FirstName"] as string ?? string.Empty,
                            P_LastName = reader["P_LastName"] as string ?? string.Empty,
                            P_MiddleName = reader["P_MiddleName"] as string ?? string.Empty,
                            Medication = reader["Medication"] as string ?? string.Empty,
                            Dosage = reader["Dosage"] as string ?? string.Empty,
                            Instructions = reader["Instructions"] as string ?? string.Empty,
                        });
                    }
                    reader.Close();
                }
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                prescriptions = prescriptions
                    .Where(d => d.P_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(prescriptions.ToPagedList(pageNumber, pageSize));
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
                TempData["SuccessMessage"] = "Prescription Details have been deleted Successfully!";
                return RedirectToAction("Prescription", "Doctor");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while deleting the prescription: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);


                return RedirectToAction("Prescription", "Doctor");
            }
        }

        public ActionResult Add_Prescription()
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");

            int doctorId = Convert.ToInt32(Session["Doctor_ID"]);
            List<SelectListItem> patients = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetPatientsByDoctor", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            patients.Add(new SelectListItem
                            {
                                Value = reader["Patient_Id"].ToString(),
                                Text = (reader["P_FirstName"] != DBNull.Value ? reader["P_FirstName"].ToString() : "") + " " +
                                       (reader["P_MiddleName"] != DBNull.Value ? reader["P_MiddleName"].ToString() : "") + " " +
                                       (reader["P_LastName"] != DBNull.Value ? reader["P_LastName"].ToString() : "")
                            });
                        }
                    }
                }
            }

            ViewBag.Patient_ID = patients;
            return View();
        }

        public JsonResult GetPatientsByDoctor(int Doctor_ID)
        {
            List<SelectListItem> patients = new List<SelectListItem>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GetPatientsByDoctor", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", Doctor_ID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            patients.Add(new SelectListItem
                            {
                                Value = reader["Patient_ID"].ToString(),
                                Text = reader["Text"].ToString()
                            });

                        }
                    }
                }
            }
            return Json(patients, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Add_Prescription(PrescriptionVM model)
        {
            if (Session["Doctor_ID"] == null)
                return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AddDrPrescription", con))
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

                TempData["SuccessMessage"] = "Prescription record has been added successfully!";
                return RedirectToAction("Prescription");
            }

            ViewBag.Patient_ID = GetPatients();
            return View(model);
        }
    }
}