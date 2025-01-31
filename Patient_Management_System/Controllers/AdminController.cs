using Microsoft.Ajax.Utilities;
using Patient_Management_System.Models;
using Patient_Management_System.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Patient_Management_System.Controllers
{
    public class AdminController : Controller

    {
        private readonly Patient_Management_SystemEntities  db = new Patient_Management_SystemEntities();
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
        public ActionResult Login(AdminVM adminVM)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_CheckAdminLogin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Email", adminVM.Email);
                        cmd.Parameters.AddWithValue("@Password", adminVM.Password);

                        int result = (int)cmd.ExecuteScalar();

                        if (result > 0)
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            ViewBag.Error = "Invalid email or password.";
                        }
                    }
                }
            }
            return View();
        }

        public ActionResult List_Appointment(AppointmentVM aptVM)
        {
            List<AppointmentVM> aptList = new List<AppointmentVM>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Get_Appointment", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    AppointmentVM apt = new AppointmentVM
                    {
                        Appointment_ID = reader["Appointment_ID"] != DBNull.Value ? Convert.ToInt32(reader["Appointment_ID"]) : 0,
                        Doctor_ID = reader["Doctor_ID"] != DBNull.Value ? Convert.ToInt32(reader["Doctor_ID"]) : 0,
                        Apt_Date = reader["Apt_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Apt_Date"]) : DateTime.MinValue,
                        Apt_Time = reader["Apt_Time"] != DBNull.Value ? (TimeSpan?)reader["Apt_Time"] : null,
                        Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                        Dept_ID = reader["Dept_ID"] != DBNull.Value ? Convert.ToInt32(reader["Dept_ID"]) : 0,
                        Diseases = reader["Diseases"] != DBNull.Value ? reader["Diseases"].ToString() : string.Empty,
                        Patient_ID = reader["Patient_ID"] != DBNull.Value ? Convert.ToInt32(reader["Patient_ID"]) : 0,
                        Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty,
                    };

                    aptList.Add(apt);
                }
            }

            return View(aptList);
        }

        public ActionResult List_Doctor(DoctorVM doctorVM)
        {
            List<DoctorVM> doctorList = new List<DoctorVM>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetDoctors", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DoctorVM doctor = new DoctorVM
                    {
                        Doctor_ID = Convert.ToInt32(reader["Doctor_ID"]),
                        Dr_FirstName = reader["Dr_FirstName"].ToString(),
                        Dr_LastName = reader["Dr_LastName"].ToString(),
                        Dept_ID = Convert.ToInt32(reader["Dept_ID"]),
                        Dept_Name = reader["Dept_Name"].ToString(),
                        Dr_Email = reader["Dr_Email"].ToString(),
                        Dr_Password = reader["Dr_Password"].ToString(),
                        Dr_DOB = Convert.ToDateTime(reader["Dr_DOB"]),
                        Dr_Gender = reader["Dr_Gender"].ToString(),
                        Dr_Phone = reader["Dr_Phone"].ToString(),
                        Dr_Qualification = reader["Dr_Qualification"].ToString(),
                        Dr_Address = reader["Dr_Address"].ToString(),
                        Dr_City = reader["Dr_City"].ToString(),
                        Dr_State = reader["Dr_State"].ToString(),
                        Dr_Pincode = Convert.ToInt32(reader["Dr_Pincode"]),
                        Dr_ImagePath = reader["Dr_ImagePath"].ToString(),
                        Fees = reader["Dr_Pincode"] != DBNull.Value ? Convert.ToInt32(reader["Dr_Pincode"]) : 0, // Default to 0 if DBNull
                        Dr_Status = reader["Dr_Status"].ToString(),
                    };
                    doctorList.Add(doctor);
                }
            }
            return View(doctorList);
        }

        public ActionResult List_Patient(PatientsTbl patients)
        {
            List<PatientsTbl> patientList = new List<PatientsTbl>();

           
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_GetPatients", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PatientsTbl patient = new PatientsTbl
                    {
                        Patient_Id = Convert.ToInt32(reader["Patient_Id"]),
                        P_FirstName = reader["P_FirstName"].ToString(),
                        P_MiddleName = reader["P_MiddleName"].ToString(),
                        P_LastName = reader["P_LastName"].ToString(),
                        P_Gender = reader["P_Gender"].ToString(),
                        P_DOB = Convert.ToDateTime(reader["P_DOB"]),
                        P_Email = reader["P_Email"].ToString(),
                        P_Phone = reader["P_Phone"].ToString(),
                        P_BloodGrp = reader["P_BloodGrp"].ToString(),
                        P_Address = reader["P_Address"].ToString(),
                        P_City = reader["P_City"].ToString(),
                        P_State = reader["P_State"].ToString(),
                        P_Pincode = reader["P_Pincode"].ToString(),
                        P_Message = reader["P_Message"].ToString(),
                        P_Image = reader["P_Image"].ToString(),
                        P_Password = reader["P_Password"].ToString()
                    };
                    patientList.Add(patient);
                }
            }
            return View(patientList);
        }
        public ActionResult List_Department(DepartmentVM departmentVM)
        {
            List<DepartmentVM> departmentList = new List<DepartmentVM>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Get_Dept", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DepartmentVM department = new DepartmentVM
                    {
                        Dept_ID = Convert.ToInt32(reader["Dept_ID"]),
                        Dept_Name = reader["Dept_Name"].ToString(),

                    };
                    departmentList.Add(department);
                }
            }
            return View(departmentList);
        }
        public string UploadImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                string imagePath = "~/Content/UploadedImages/" + fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

                try
                {
                    file.SaveAs(Server.MapPath(imagePath));
                    return imagePath;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error saving image: " + ex.Message);
                    return null;
                }
            }
            return null;
        }

    

        [HttpGet]
        public ActionResult Add_Appointment()
        {  
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName");
            //ViewBag.Schedule_ID = new SelectList(db.ScheduleTbls, "Schedule_ID", "Available_Date");
            ViewBag.TimeSlots = GetTimeSlots();
            return View();
        }
        private List<SelectListItem> GetTimeSlots()
        {
            List<SelectListItem> timeSlots = new List<SelectListItem>();
            TimeSpan startTime = new TimeSpan(9, 30, 0); // 9:30 AM
            TimeSpan endTime = new TimeSpan(18, 30, 0); // 11:30 PM

            while (startTime <= endTime)
            {
                string timeValue = startTime.ToString(@"hh\:mm");
                timeSlots.Add(new SelectListItem { Value = timeValue, Text = timeValue });
                startTime = startTime.Add(new TimeSpan(0, 30, 0));
            }
            return timeSlots;
        }

        [HttpPost]
        public ActionResult Add_Appointment(AppointmentVM aptVM)
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
                            cmd.Parameters.AddWithValue("@Doctor_ID", aptVM.Doctor_ID);
                            cmd.Parameters.AddWithValue("@Dept_ID", aptVM.Dept_ID);
                            //cmd.Parameters.AddWithValue("@Schedule_ID", aptVM.Schedule_ID);
                            cmd.Parameters.AddWithValue("@Apt_Date", aptVM.Apt_Date);
                            cmd.Parameters.AddWithValue("@Apt_Time", aptVM.Apt_Time);
                            cmd.Parameters.AddWithValue("@Description", aptVM.Description);
                            cmd.Parameters.AddWithValue("@Email", aptVM.Email);
                            cmd.Parameters.AddWithValue("@Diseases", aptVM.Diseases);

                            cmd.ExecuteNonQuery(); // Ensure the command is executed
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

            // Repopulating dropdowns before returning the view
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", aptVM.Doctor_ID);

            //ViewBag.Schedule_ID = new SelectList(db.ScheduleTbls, "Schedule_ID", "Available_Date", aptVM.Schedule_ID);
            ViewBag.TimeSlots = GetTimeSlots();

            return View(aptVM);
        }





        [HttpGet]
        public ActionResult Add_Doctor()
        {
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");

            return View();
        }
        [HttpPost]
        public ActionResult Add_Doctor(DoctorVM doctorVM, HttpPostedFileBase Dr_ImagePath)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            if (ModelState.IsValid)
            {
                string imagePath = UploadImage(Dr_ImagePath);

                if (imagePath == null)
                {
                    ViewBag.Error = "Error uploading image.";
                    return View(doctorVM);
                }
                try
                {
                    using (conn)
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_Add_Doctor", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Dr_FirstName", doctorVM.Dr_FirstName);
                            cmd.Parameters.AddWithValue("@Dr_LastName", doctorVM.Dr_LastName);
                            cmd.Parameters.AddWithValue("@Dr_Qualification", doctorVM.Dr_Qualification);
                            cmd.Parameters.AddWithValue("@Dr_Email", doctorVM.Dr_Email);
                            cmd.Parameters.AddWithValue("@Dr_Password", doctorVM.Dr_Password);
                            cmd.Parameters.AddWithValue("@Dr_DOB", doctorVM.Dr_DOB ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Dr_Gender", doctorVM.Dr_Gender);
                            cmd.Parameters.AddWithValue("@Dr_Address", doctorVM.Dr_Address);
                            cmd.Parameters.AddWithValue("@Dr_City", doctorVM.Dr_City);
                            cmd.Parameters.AddWithValue("@Dr_State", doctorVM.Dr_State);
                            cmd.Parameters.AddWithValue("@Dr_Pincode", doctorVM.Dr_Pincode);
                            cmd.Parameters.AddWithValue("@Dr_Phone", doctorVM.Dr_Phone);
                            cmd.Parameters.AddWithValue("@Dr_ImagePath", imagePath ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Dr_Status", doctorVM.Dr_Status);
                            cmd.Parameters.AddWithValue("@Fees", doctorVM.Fees);
                            cmd.Parameters.AddWithValue("@Dept_ID", doctorVM.Dept_ID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return RedirectToAction("List_Doctor", "Admin");
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
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", doctorVM.Dept_ID);
           
            return View(doctorVM);
        }
        [HttpGet]
        public ActionResult Add_Patient()
        {
            return View();
        }
        public ActionResult Add_Patient(PatientVM patients, HttpPostedFileBase P_Image)
        { 
            SqlConnection conn = new SqlConnection(connectionString);
            if (ModelState.IsValid)
            {
                string imagePath = UploadImage(P_Image); // Use helper method

                if (imagePath == null)
                {
                    ViewBag.Error = "Error uploading image.";
                    return View(patients);
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
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }
            return View(patients);
        }
        public ActionResult Schedule()
        {
            var scheduleTbls = db.ScheduleTbls.Include(s => s.DepartmentTbl).Include(s => s.DoctorTbl);
            return View(scheduleTbls.ToList());
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
                return RedirectToAction("Schedule");
            }

            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", scheduleTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(
       db.DoctorTbls.Select(d => new {
           Doctor_ID = d.Doctor_ID,
           FullName = d.Dr_FirstName + " " + d.Dr_LastName
       }), "Doctor_ID", "FullName", scheduleTbl.Doctor_ID
   ); return View(scheduleTbl);
        }

        [HttpGet]
        public ActionResult Add_Department()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add_Department(DepartmentVM departmentVM)
        {

            SqlConnection conn = new SqlConnection(connectionString);
            if (ModelState.IsValid)
            {
                try
                {
                    using (conn)
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_Add_Department", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Dept_Name", departmentVM.Dept_Name);
                            cmd.ExecuteNonQuery();
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
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }
            return View(departmentVM);
        }

        [HttpGet]
        public ActionResult Edit_Doctor(int doctorId)
        {
            var doctor = db.DoctorTbls.Where(d => d.Doctor_ID == doctorId).FirstOrDefault();
            if (doctor == null)
            {
                TempData["Error"] = "Doctor not found.";
                return RedirectToAction("List_Doctor");
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
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", doctor.Dept_ID);

            return View(doctorVM);
        }

        [HttpPost]
        public ActionResult Edit_Doctor(DoctorVM doctor, HttpPostedFileBase Dr_ImagePath)
        {
            try
            {
                if (Dr_ImagePath != null && Dr_ImagePath.ContentLength > 0)
                {
                    string uploadPath = Server.MapPath("~/Content/UploadedImages/");
                    string fileName = Path.GetFileNameWithoutExtension(Dr_ImagePath.FileName);
                    string extension = Path.GetExtension(Dr_ImagePath.FileName);
                    string newImagePath = "~/Content/UploadedImages/" + fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                    try
                    {
                        // Save the image
                        Dr_ImagePath.SaveAs(Server.MapPath(newImagePath));
                        doctor.Dr_ImagePath = newImagePath; // Update the doctor object with the new image path
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

                        // Add parameters
                        cmd.Parameters.AddWithValue("@Doctor_ID", doctor.Doctor_ID);
                        cmd.Parameters.AddWithValue("@Dr_FirstName", doctor.Dr_FirstName);
                        cmd.Parameters.AddWithValue("@Dr_LastName", doctor.Dr_LastName);
                        cmd.Parameters.AddWithValue("@Dr_Email", doctor.Dr_Email);
                        cmd.Parameters.AddWithValue("@Dr_Password", doctor.Dr_Password);
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

                        // Open connection and execute command
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Message"] = "Doctor record updated successfully.";
                return RedirectToAction("Index"); // Redirect to index or any other action
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(doctor);
            }
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", doctor.Dept_ID);
            return View(doctor);
        }

        [HttpGet]
        public ActionResult Edit_Appointment(int aptId)
        {
            var appointment = db.AppointmentTbls.FirstOrDefault(d => d.Appointment_ID == aptId);
            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToAction("List_Appointment");
            }

            AppointmentVM aptVM = new AppointmentVM
            {
                Appointment_ID = appointment.Appointment_ID,
                Doctor_ID = appointment.Doctor_ID,
                Description = appointment.Description,
                Dept_ID = appointment.Dept_ID,
                Email = appointment.Email,
                Diseases = appointment.Diseases,
                Apt_Date = appointment.Apt_Date,
                Apt_Time = appointment.Apt_Time,

            };

            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", aptVM.Doctor_ID);
            ViewBag.Schedule_ID = new SelectList(db.ScheduleTbls, "Schedule_ID", "Available_Date", aptVM.Schedule_ID);
            ViewBag.TimeSlots = GetTimeSlots();

            return View(aptVM);
        }


        [HttpPost]
        public ActionResult Edit_Appointment(AppointmentVM aptVM)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Edit_Appointment", con))

                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Appointment_ID", aptVM.Appointment_ID);
                        cmd.Parameters.AddWithValue("@Doctor_ID", aptVM.Doctor_ID);
                        cmd.Parameters.AddWithValue("@Description", aptVM.Description);
                        cmd.Parameters.AddWithValue("@Dept_ID", aptVM.Dept_ID);
                        cmd.Parameters.AddWithValue("@Email", aptVM.Email);
                        cmd.Parameters.AddWithValue("@Diseases", aptVM.Diseases);
                        cmd.Parameters.AddWithValue("@Apt_Time", aptVM.Apt_Time);
                        cmd.Parameters.AddWithValue("@Apt_Date", aptVM.Apt_Date);


                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Message"] = "Doctor record updated successfully.";
                return RedirectToAction("List_Appointment"); // Redirect to index or any other action
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            // Ensure ViewBag is set before returning the view
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", aptVM.Doctor_ID);
            ViewBag.Schedule_ID = new SelectList(db.ScheduleTbls, "Schedule_ID", "Available_Date", aptVM.Schedule_ID);
            ViewBag.TimeSlots = GetTimeSlots();

            return View(aptVM);
        }

        [HttpGet]
        public ActionResult Edit_Patient(int patientId)
        {
            var patient = db.PatientsTbls.Where(d => d.Patient_Id == patientId).FirstOrDefault();
            if (patient == null)
            {
                TempData["Error"] = "Patient not found.";
                return RedirectToAction("Index");
            }

            PatientVM patientVM = new PatientVM
            {
                Patient_Id = patient.Patient_Id,
                P_FirstName = patient.P_FirstName,
                P_MiddleName = patient.P_MiddleName,
                P_LastName = patient.P_LastName,
                P_Gender = patient.P_Gender,
                P_DOB = patient.P_DOB,
                P_Email = patient.P_Email,
                P_Phone = patient.P_Phone,
                P_BloodGrp = patient.P_BloodGrp,
                P_Address = patient.P_Address,
                P_City = patient.P_City,
                P_State = patient.P_State,
                P_Pincode = patient.P_Pincode,
                P_Password = patient.P_Password,
                P_Message = patient.P_Message,
                P_Image = patient.P_Image,

            };
            return View(patientVM);
        }

        [HttpPost]
        public ActionResult Edit_Patient(PatientVM patientVM, HttpPostedFileBase ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string imagePath = UploadImage(ImageFile); // Use helper method

                    if (imagePath == null)
                    {
                        ViewBag.Error = "Error uploading image.";
                        return View(patientVM);
                    }

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_Edit_Patient", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Patient_Id", patientVM.Patient_Id);
                            cmd.Parameters.AddWithValue("@P_FirstName", patientVM.P_FirstName);
                            cmd.Parameters.AddWithValue("@P_MiddleName", patientVM.P_MiddleName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_LastName", patientVM.P_LastName ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_Gender", patientVM.P_Gender ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_DOB", (object)patientVM.P_DOB ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_Email", patientVM.P_Email ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_Phone", patientVM.P_Phone ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_BloodGrp", patientVM.P_BloodGrp ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_Address", patientVM.P_Address ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_City", patientVM.P_City ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_State", patientVM.P_State ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_Pincode", patientVM.P_Pincode ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_Message", patientVM.P_Message ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@P_Image", ImageFile ?? (object)DBNull.Value);


                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    TempData["Message"] = "Patient record updated successfully.";
                    return RedirectToAction("List_Patient");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
                return View(patientVM);
            }
            return View(patientVM);
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
                return RedirectToAction("Index");
            }
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", scheduleTbl.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", scheduleTbl.Doctor_ID);
            return View(scheduleTbl);
        }
        [HttpGet]
        public ActionResult Edit_Department(int deptId)
        {
            var department = db.DepartmentTbls.Where(d => d.Dept_ID == deptId).FirstOrDefault();
            if (department == null)
            {
                TempData["Error"] = "Department not found.";
                return RedirectToAction("Index");
            }

            DepartmentVM departmentVM = new DepartmentVM
            {
                Dept_ID = department.Dept_ID,
                Dept_Name = department.Dept_Name
            };


            return View(departmentVM);
        }

        [HttpPost]
        public ActionResult Edit_Department(DepartmentVM departmentVM)
        {
            try
            {

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Edit_Department", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Dept_ID", departmentVM.Dept_ID);
                        cmd.Parameters.AddWithValue("@Dept_Name", departmentVM.Dept_Name);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Message"] = "Dept record updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
                return View(departmentVM);
            }
        }

        public ActionResult Delete_Doctor(int doctorId)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                using (conn)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Delete_Doctor", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("List_Doctor", "Admin");
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }

            return RedirectToAction("List_Doctor", "Admin");
        }

        public ActionResult Delete_Patient(int patientId)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                using (conn)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_DeletePatient", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add the parameter for Doctor_ID
                        cmd.Parameters.AddWithValue("@Patient_Id", patientId);

                        // Execute the stored procedure
                        cmd.ExecuteNonQuery();
                    }
                }


                return RedirectToAction("List_Patient", "Admin");
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }

            return RedirectToAction("List_Patient", "Admin");
        }

        public ActionResult Delete_Appointment(int aptId)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                using (conn)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Delete_Appointment", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Appointment_ID", aptId);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("List_Appointment", "Admin");
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }

            return RedirectToAction("List_Appointment", "Admin");
        }

        public ActionResult Delete_Department(int DeptId)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                using (conn)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Delete_Dept", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add the parameter for Doctor_ID
                        cmd.Parameters.AddWithValue("@Dept_ID", DeptId);

                        // Execute the stored procedure
                        cmd.ExecuteNonQuery();
                    }
                }


                return RedirectToAction("List_Department", "Admin");
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }

            return RedirectToAction("List_Department", "Admin");
        }

        public ActionResult Delete_Schedule(int? id)
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
            return View(scheduleTbl);
        }

        [HttpPost, ActionName("Delete_Schedule")]
        public ActionResult DeleteConfirmed(int id)
        {
            ScheduleTbl scheduleTbl = db.ScheduleTbls.Find(id);
            db.ScheduleTbls.Remove(scheduleTbl);
            db.SaveChanges();
            return RedirectToAction("Schedule");
        }

    }
}
