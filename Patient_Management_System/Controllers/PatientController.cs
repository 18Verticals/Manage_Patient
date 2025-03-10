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
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Web.Security;
namespace Patient_Management_System.Controllers
{
    public class PatientController : Controller
    {
        private readonly Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(PatientVM patients, HttpPostedFileBase P_Image)
        {
            string str = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            SqlConnection conn = new SqlConnection(str);

            if (ModelState.IsValid)
            {
                string imagePath = null;

                if (P_Image != null && P_Image.ContentLength > 0)
                {
                    string uploadPath = Server.MapPath("~/Content/UploadedImages/");
                    string fileName = Path.GetFileNameWithoutExtension(P_Image.FileName);
                    string extension = Path.GetExtension(P_Image.FileName);
                    imagePath = "~/Content/UploadedImages/" + fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    System.Diagnostics.Debug.WriteLine("Image Path: " + imagePath);
                    try
                    {
                        P_Image.SaveAs(Server.MapPath(imagePath));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error saving image: " + ex.Message);
                        return View(patients);
                    }
                }
                try
                {
                    using (conn)
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand("sp_Add_Patients", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@P_FirstName", patients.P_FirstName);
                            cmd.Parameters.AddWithValue("@P_MiddleName", patients.P_MiddleName);
                            cmd.Parameters.AddWithValue("@P_LastName", patients.P_LastName);
                            cmd.Parameters.AddWithValue("@P_Gender", patients.P_Gender);
                            cmd.Parameters.AddWithValue("@P_DOB", patients.P_DOB ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_Email", patients.P_Email);
                            cmd.Parameters.AddWithValue("@P_Phone", patients.P_Phone);
                            cmd.Parameters.AddWithValue("@P_BloodGrp", patients.P_BloodGrp);
                            cmd.Parameters.AddWithValue("@P_Address", patients.P_Address);
                            cmd.Parameters.AddWithValue("@P_City", patients.P_City);
                            cmd.Parameters.AddWithValue("@P_State", patients.P_State);
                            cmd.Parameters.AddWithValue("@P_Pincode", patients.P_Pincode);
                            cmd.Parameters.AddWithValue("@P_Message", patients.P_Message);
                            cmd.Parameters.AddWithValue("@P_Image", imagePath ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_Password", patients.P_Password);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return RedirectToAction("Login");
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        ViewBag.Message = "User already registered";
                    }
                    else if (ex.Message.Contains("Email already registered."))
                    {
                        ViewBag.Message = "The Email you entered is already registered.";
                    }
                    else
                    {
                        ViewBag.Message = "An error occurred: " + ex.Message;
                    }
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
            return View(patients);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            PatientVM patient = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_LoginInfo", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        patient = new PatientVM
                        {
                            Patient_Id = Convert.ToInt32(reader["Patient_Id"]),
                            P_FirstName = reader["P_FirstName"].ToString(),
                            P_Email = reader["P_Email"].ToString()
                        };
                    }

                    reader.Close();
                }
            }
            if (patient != null)
            {
                Session["Patient_Id"] = patient.Patient_Id;
                Session["P_FirstName"] = patient.P_FirstName;
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Invalid email or password!";
            return View();
        }

        [HttpGet]
        public ActionResult Appointment()
        {
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
            ViewBag.Doctor_ID = new SelectList(new List<SelectListItem>(), "Value", "Text");
            ViewBag.TimeSlots = GetTimeSlots();
            return View();
        }

        [HttpPost]
        public ActionResult Appointment(AppointmentVM aptVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
                ViewBag.Doctor_ID = new SelectList(db.DoctorTbls.Where(d => d.Dept_ID == aptVM.Dept_ID), "Doctor_ID", "Dr_FirstName", aptVM.Doctor_ID);
                return View(aptVM);
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[sp_Demo_Book_Appointment]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", aptVM.Doctor_ID);
                    cmd.Parameters.AddWithValue("@Dept_ID", aptVM.Dept_ID);
                    cmd.Parameters.AddWithValue("@Apt_Date", aptVM.Apt_Date);
                    cmd.Parameters.AddWithValue("@Apt_Time", aptVM.Apt_Time);
                    cmd.Parameters.AddWithValue("@Description", aptVM.Description);
                    cmd.Parameters.AddWithValue("@Phone", aptVM.Phone);
                    cmd.Parameters.AddWithValue("@Diseases", aptVM.Diseases);

                    SqlParameter returnValue = new SqlParameter
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    cmd.Parameters.Add(returnValue);


                    SqlParameter emailParam = new SqlParameter("@PatientEmail", SqlDbType.NVarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(emailParam);

                    SqlParameter patientNameParam = new SqlParameter("@PatientName", SqlDbType.NVarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(patientNameParam);

                    SqlParameter doctorNameParam = new SqlParameter("@DoctorName", SqlDbType.NVarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(doctorNameParam);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    int result = (returnValue.Value != DBNull.Value) ? Convert.ToInt32(returnValue.Value) : -2;
                    string patientEmail = emailParam.Value.ToString();
                    string patientName = patientNameParam.Value?.ToString() ?? "Patient";
                    string doctorName = doctorNameParam.Value?.ToString() ?? "Doctor";
                    if (result == 1)
                    {

                        TempData["ErrorMessage"] = "Appointment booked successfully!";

                        SendEmailNotification(patientEmail, patientName, doctorName, aptVM);
                    }
                    else if (result == 0)
                    {
                        TempData["ErrorMessage"] = "This time slot is already booked!";
                    }
                    else if (result == -1)
                    {
                        TempData["ErrorMessage"] = "No patient exists with this phone number.";
                    }
                    else if (result == -2)
                    {
                        TempData["ErrorMessage"] = "You have already booked an appointment with this doctor on the same day.";
                    }

                    else if (result == -3)
                    {
                        TempData["ErrorMessage"] = "No Avaible Doctor This Date ";
                    }

                    else if (result == -4)
                    {
                        TempData["ErrorMessage"] = "Time Slot Not available ";
                    }
                    else if(result == -5)
                    {
                        TempData["ErrorMessage"] = "Doctor is not available ";

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An unexpected error occurred.";
                    }
                }
            }
            return RedirectToAction("Appointment");
        }


        private void SendEmailNotification(string email, string patientName, string doctorName, AppointmentVM aptVM)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("No email found for the patient.");
                    return;
                }

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("hemangkanzariya00@gmail.com"),
                    Subject = "Appointment Confirmation",
                    Body = $"Dear {patientName},\n\n" +
                           $"Your appointment has been confirmed with Dr. {doctorName} on {aptVM.Apt_Date:dd-MM-yyyy} at {aptVM.Apt_Time}.\n\n" +
                           $"Description: {aptVM.Description}\n\n" +
                           $"Thank you!\n\nBest Regards,\nLiveDoc Multispecialist Hospital\n\n" +
                           $"Any Query? Please Contact Us: 70465 90890",
                    IsBodyHtml = false
                };
                mail.To.Add(email);

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("hemangkanzariya00@gmail.com", "lqri ukod qdsl qyfx")
                };
                smtp.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
            }
        }

        [HttpGet]
        public JsonResult GetDoctorsByDepartment(int deptId)
        {
            var doctors = db.DoctorTbls.Where(d => d.Dept_ID == deptId)
                                       .Select(d => new { d.Doctor_ID, d.Dr_FirstName })
                                       .ToList();
            return Json(doctors, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentByDoctor(int doctorId)
        {
            var doctor = db.DoctorTbls.Where(d => d.Doctor_ID == doctorId)
                                      .Select(d => new { Dept_ID = d.Dept_ID })
                                      .FirstOrDefault();
            return Json(doctor, JsonRequestBehavior.AllowGet);
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
        private void SendEmailNotification(string email, AppointmentVM aptVM)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("No email found for the patient.");
                    return;
                }

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("hemangkanzariya00@gmail.com"),
                    Subject = "Appointment Confirmation",
                    Body = $"Dear Patient,\n\nYour appointment has been confirmed with Dr. {aptVM.Doctor_ID} on  {aptVM.Apt_Date:dd-MM-yyyy} at {aptVM.Apt_Time}.\n\nDescription: {aptVM.Description}\n\nThank you!\n\nBest Regards,\nLiveDoc Multispecialist Hospital\n\n Any Query? Please Contact Us:70465 90890",
                    IsBodyHtml = false
                };

                mail.To.Add(email);

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("hemangkanzariya00@gmail.com", "lqri ukod qdsl qyfx"), // Use App Password
                    EnableSsl = true
                };

                smtp.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
            }
        }
        public ActionResult Doctor_ViewProfile(int id)
        {
            DoctorVM doctor = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDoctorProfile", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", id);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            doctor = new DoctorVM
                            {
                                Doctor_ID = Convert.ToInt32(dr["Doctor_ID"]),
                                Dr_FirstName = dr["Dr_FirstName"].ToString(),
                                Dr_LastName = dr["Dr_LastName"].ToString(),
                                Dr_Status = dr["Dr_Status"].ToString(),
                                Dept_Name = dr["Dept_Name"].ToString(),
                                Dr_Qualification = dr["Dr_Qualification"].ToString(),
                                Dr_ImagePath = dr["Dr_ImagePath"].ToString(),
                                Fees = Convert.ToInt32(dr["Fees"]),
                                Dr_Email = dr["Dr_Email"].ToString(),
                                Dr_Phone= dr["Dr_Phone"].ToString(),
                            };
                        }
                    }
                }
            }

            if (doctor == null)
            {
                return HttpNotFound("Doctor not found.");
            }
            return View(doctor);
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
                            int doctorId = Convert.ToInt32(dr["Doctor_ID"]);
                            var doctor = doctors.FirstOrDefault(d => d.Doctor_ID == doctorId);

                            if (doctor == null)
                            {
                                doctor = new DoctorVM
                                {
                                    Doctor_ID = doctorId,
                                    Dr_FirstName = dr["Dr_FirstName"].ToString(),
                                    Dr_LastName = dr["Dr_LastName"].ToString(),
                                    Dept_Name = dr["Dept_Name"].ToString(),
                                    Dr_Qualification = dr["Dr_Qualification"].ToString(),
                                    Dr_ImagePath = dr["Dr_ImagePath"].ToString(),
                                    Fees = Convert.ToInt32(dr["Fees"]),
                                    Available_Date = new List<DateTime>(),
                                    DateTimeSlots = new Dictionary<DateTime, (TimeSpan, TimeSpan, string)>()
                                };
                                doctors.Add(doctor);
                            }

                            if (dr["Available_Date"] != DBNull.Value)
                            {
                                DateTime availableDate = Convert.ToDateTime(dr["Available_Date"]);
                                doctor.Available_Date.Add(availableDate);

                                TimeSpan startTime = dr["Start_Time"] != DBNull.Value ? (TimeSpan)dr["Start_Time"] : TimeSpan.Zero;
                                TimeSpan endTime = dr["End_Time"] != DBNull.Value ? (TimeSpan)dr["End_Time"] : TimeSpan.Zero;
                                string status = dr["Status"] != DBNull.Value ? dr["Status"].ToString() : "Inactive";

                                doctor.DateTimeSlots[availableDate] = (startTime, endTime, status);
                            }
                        }
                    }
                }
            }
            return View(doctors);
        }

        public ActionResult Payment()
        {
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
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_Add_Contact", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Name", contact.Name);
                            cmd.Parameters.AddWithValue("@Email", contact.Email);
                            cmd.Parameters.AddWithValue("@Message", contact.Message);
                            cmd.Parameters.AddWithValue("@Phone", contact.Phone);


                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                TempData["SuccessMessage"] = "Thank You";
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.Error = "No data was inserted.";
                            }
                        }
                    }
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