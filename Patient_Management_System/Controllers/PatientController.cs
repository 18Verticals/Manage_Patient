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
                    System.Diagnostics.Debug.WriteLine("Image Path: " + imagePath); // Log image path
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

                        string checkEmailQuery = "SELECT COUNT(*) FROM PatientsTbl WHERE P_Email = @P_Email";
                        using (SqlCommand cmdCheckEmail = new SqlCommand(checkEmailQuery, conn))
                        {
                            cmdCheckEmail.Parameters.AddWithValue("@P_Email", patients.P_Email);
                            int emailExists = (int)cmdCheckEmail.ExecuteScalar();

                            if (emailExists > 0)
                            {
                                ViewBag.Error = "The email address already exists. Please choose a different email.";
                                return View(patients);
                            }
                        }
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
            return View(patients);
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(PatientsTbl patients)
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
                        using (SqlCommand cmd = new SqlCommand("sp_LoginInfo", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Email", patients.P_Email);
                            cmd.Parameters.AddWithValue("@Password", patients.P_Password);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {

                                return RedirectToAction("Index", "Home");
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
            return View(patients);
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
                using (SqlCommand cmd = new SqlCommand("[sp_Book_Appointment]", con))
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
                                return RedirectToAction("Index", "Admin");
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