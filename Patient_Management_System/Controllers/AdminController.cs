﻿using Microsoft.Ajax.Utilities;
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
        public ActionResult List_Appointment(AppointmentVM aptVM)//Changes
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

                        Appointment_ID = reader["Appointment_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Appointment_ID"]),

                        Doctor_ID = reader["Doctor_ID"] != DBNull.Value ? Convert.ToInt32(reader["Doctor_ID"]) : (int?)null,
                        Dr_FirstName = reader["Dr_FirstName"].ToString(),

                        Patient_ID = reader["Patient_ID"] != DBNull.Value ? Convert.ToInt32(reader["Patient_ID"]) : (int?)null,
                        Dept_ID = reader["Dept_ID"] != DBNull.Value ? Convert.ToInt32(reader["Dept_ID"]) : (int?)null,
                        Dept_Name = reader["Dept_Name"].ToString(),
                        Apt_Date = reader["Apt_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Apt_Date"]) : DateTime.MinValue,
                        Apt_Time = reader["Apt_Time"] != DBNull.Value ? (TimeSpan?)reader["Apt_Time"] : null,
                        Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,  
                        Diseases = reader["Diseases"] != DBNull.Value ? reader["Diseases"].ToString() : string.Empty,
                        Phone= reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : string.Empty,
                    };
                    aptList.Add(apt);
                }
            }
            return View(aptList);
        }

        public ActionResult List_Prescription(PrescriptionVM prescVM)
        {
            List<PrescriptionVM> PrescList = new List<PrescriptionVM>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Get_Prescription", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PrescriptionVM presc = new PrescriptionVM
                    {
                        Presc_ID = Convert.ToInt32(reader["Presc_ID"]),
                        Doctor_ID = Convert.ToInt32(reader["Doctor_ID"]),
                        Dr_FirstName = reader["Dr_FirstName"].ToString(),
                        Patient_ID = Convert.ToInt32(reader["Patient_ID"]),
                       // P_FirstName = reader["P_FirstName"].ToString(),
                        DateIssued = Convert.ToDateTime(reader["DateIssued"]),
                        Medication = reader["Medication"].ToString(),
                        Dosage = reader["Dosage"].ToString(),
                        Instructions = reader["Dosage"].ToString(),
                    };
                    PrescList.Add(presc);
                }
            }
            return View(PrescList);
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

      
        public ActionResult List_Contact()
        {
            return View(db.ContactUsTbls.ToList());
        }

        public ActionResult List_Payment(int? paymentId = null)
        {
            List<PaymentVM> PaymentList = new List<PaymentVM>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Get_Payment", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PaymentVM payment = new PaymentVM
                    {
                        Payment_ID = Convert.ToInt32(reader["Payment_ID"]),
                        Patient_ID = Convert.ToInt32(reader["Patient_ID"]),
                        Amount = Convert.ToInt32(reader["Amount"]),
                        PaymentMethod = reader["PaymentMethod"].ToString(),
                        PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                        Status = reader["Status"].ToString(),
                        Remarks = reader["Remarks"].ToString(),
                    };
                    PaymentList.Add(payment);
                }
            }
            return View(PaymentList);
        }

       

        [HttpGet]
        public ActionResult Add_Appointment()
        {
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName");
            ViewBag.TimeSlots = GetTimeSlots();
            return View();
        }

        [HttpPost]
        public ActionResult Add_Appointment(AppointmentVM aptVM)
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

                    // Pass required parameters
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
                        TempData["Message"] = "Appointment booked successfully!";
                        return RedirectToAction("List_Appointment");  // Redirect after successful booking
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
            return View(aptVM); // Return to form if there's an issue
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


        [HttpGet]
        public ActionResult Add_Prescription()
        {
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName");
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_Id", "P_FirstName");

            return View();
        }
        [HttpPost]
        public ActionResult Add_Prescription(PrescriptionVM prescVM)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            if (ModelState.IsValid)
            {
                try
                {
                    using (conn)
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_Add_Prescription", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Doctor_ID", prescVM.Doctor_ID);
                            cmd.Parameters.AddWithValue("@Patient_ID", prescVM.Patient_ID);
                            cmd.Parameters.AddWithValue("@DateIssued", prescVM.DateIssued);
                            cmd.Parameters.AddWithValue("@Medication", prescVM.Medication);
                            cmd.Parameters.AddWithValue("@Dosage", prescVM.Dosage);
                            cmd.Parameters.AddWithValue("@Instructions", prescVM.Instructions);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return RedirectToAction("List_Prescription", "Admin");
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
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", prescVM.Doctor_ID);
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_ID", "P_FirstName", prescVM.Patient_ID);

            return View(prescVM);
        }
        public string UploadImage(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                string imagePath = "~/Content/UploadedImages/" + fileName + DateTime.Now.ToString("ddMMHHyyyy") + extension;

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
                catch (SqlException ex)
                {
                    // Check if it's the unique email error
                    if (ex.Message.Contains("Email already exists"))
                    {
                        ViewBag.Error = "The email you entered is already associated with another doctor. Please use a different email.";
                    }
                    else
                    {
                        ViewBag.Error = "An error occurred: " + ex.Message;
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
                string imagePath = UploadImage(P_Image);

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
           Doctor_ID = d.Doctor_ID,FullName = d.Dr_FirstName + " " + d.Dr_LastName }),"Doctor_ID", "FullName", scheduleTbl.Doctor_ID); 
            return View(scheduleTbl);
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
                    return RedirectToAction("List_Department", "Admin");
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
            return View(departmentVM);
        }
       
        [HttpGet]
        public ActionResult Add_Payment()
        {
            ViewBag.Patient_Id = new SelectList(db.PatientsTbls, "Patient_Id ", "P_FirstName");
            return View();
        }

        [HttpPost]
        public ActionResult Add_Payment(PaymentVM paymentVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Patient_Id = new SelectList(db.PatientsTbls, "Patient_Id", "P_FirstName", paymentVM.Patient_ID);
                return View(paymentVM);
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[sp_Add_Payment]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Patient_ID", paymentVM.Patient_ID);
                        cmd.Parameters.AddWithValue("@Amount", paymentVM.Amount);
                        cmd.Parameters.AddWithValue("@PaymentMethod", paymentVM.PaymentMethod);
                        cmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Status", paymentVM.Status);
                        cmd.Parameters.AddWithValue("@Remarks", paymentVM.Remarks);

                        cmd.ExecuteNonQuery();
                    }
                    conn.Close(); 
                }
            }
            catch (SqlException sqlEx)
            {
                ViewBag.Error = "SQL Error: " + sqlEx.Message;
                System.Diagnostics.Debug.WriteLine("SQL Error: " + sqlEx.Message);
                return View(paymentVM);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("General Error: " + ex.Message);
                return View(paymentVM);
            }

            return RedirectToAction("List_Payment"); 
        }

        //[HttpGet]
        //public ActionResult Add_Contact()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Add_Contact([Bind(Include = "Feedback_Id,Name,Email,Message,Phone")] ContactUsTbl contactUsTbl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.ContactUsTbls.Add(contactUsTbl);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(contactUsTbl);
        //}
        

        
     
        //[HttpGet]
        //public ActionResult Add_Contact()
        //{
        //    return View();
        //}

  
        //[HttpPost]
        //public ActionResult Add_Contact(string name, string email, string message, string phone)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            using (SqlCommand cmd = new SqlCommand("sp_Add_Contact", conn))
        //            {
        //                cmd.CommandType = CommandType.StoredProcedure;
        //                cmd.Parameters.AddWithValue("@Name", name);
        //                cmd.Parameters.AddWithValue("@Email", email);
        //                cmd.Parameters.AddWithValue("@Message", message);
        //                cmd.Parameters.AddWithValue("@Phone", phone);

        //                conn.Open();
        //                cmd.ExecuteNonQuery();
        //            }
        //        }

        //        ViewBag.Message = "Contact details added successfully!";
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.Message = "Error: " + ex.Message;
        //    }

        //    return View();
        //}







        [HttpGet]
        public ActionResult Edit_Contact(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsTbl contactUsTbl = db.ContactUsTbls.Find(id);
            if (contactUsTbl == null)
            {
                return HttpNotFound();
            }
            return View(contactUsTbl);
        }
        
        [HttpPost]
        public ActionResult Edit_Contact([Bind(Include = "Feedback_Id,Name,Email,Message,Phone")] ContactUsTbl contactUsTbl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contactUsTbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactUsTbl);
        }

        [HttpGet]
        public ActionResult Edit_Payment(int paymentId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[sp_Edit_Payment]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Payment_ID ", paymentId);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PaymentVM paymentVM = new PaymentVM
                            {
                                Payment_ID = Convert.ToInt32(reader["Payment_ID"]),
                                Patient_ID = Convert.ToInt32(reader["Patient_ID"]),
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                PaymentMethod = reader["PaymentMethod"].ToString(),
                                PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                                Status = reader["Status"].ToString(),
                                Remarks = reader["Remarks"]?.ToString()
                            };
                            return View(paymentVM);
                        }
                    }
                }
            }
            TempData["Error"] = "Payment record not found.";
            return RedirectToAction("List_Payments");
        }

        [HttpPost]
        public ActionResult Edit_Payment(PaymentVM paymentVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_Edit_Payment", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;



                            cmd.Parameters.AddWithValue("@Payment_ID", paymentVM.Payment_ID);
                            cmd.Parameters.AddWithValue("@Patient_ID", paymentVM.Patient_ID);
                            cmd.Parameters.AddWithValue("@Amount", paymentVM.Amount);
                            cmd.Parameters.AddWithValue("@PaymentMethod", paymentVM.PaymentMethod);
                            cmd.Parameters.AddWithValue("@PaymentDate", paymentVM.PaymentDate);
                            cmd.Parameters.AddWithValue("@Status", paymentVM.Status);
                            cmd.Parameters.AddWithValue("@Remarks", paymentVM.Remarks ?? (object)DBNull.Value);


                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    TempData["Message"] = "Payment record updated successfully.";
                    return RedirectToAction("List_Payments");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
            }
            return View(paymentVM);
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
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(doctor);
            }
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
                Phone = appointment.Phone,
                Diseases = appointment.Diseases,
                Apt_Date = appointment.Apt_Date,
                Apt_Time = appointment.Apt_Time,

            };

            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", aptVM.Doctor_ID);
            // ViewBag.Schedule_ID = new SelectList(db.ScheduleTbls, "Schedule_ID", "Available_Date", aptVM.Schedule_ID);
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
                        cmd.Parameters.AddWithValue("@Phone", aptVM.Phone);
                        cmd.Parameters.AddWithValue("@Diseases", aptVM.Diseases);
                        cmd.Parameters.AddWithValue("@Apt_Time", aptVM.Apt_Time);
                        cmd.Parameters.AddWithValue("@Apt_Date", aptVM.Apt_Date);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Message"] = "Doctor record updated successfully.";
                return RedirectToAction("List_Appointment"); 
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", aptVM.Doctor_ID);
            ViewBag.TimeSlots = GetTimeSlots();
            return View(aptVM);
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

        [HttpGet]
        public ActionResult Edit_Prescription(int PrescId)
        {
            var presc = db.PrescriptionTbls.Where(d => d.Presc_ID == PrescId).FirstOrDefault();
            if (presc == null)
            {
                return HttpNotFound();
            }

           
         
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName");
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_Id", "P_FirstName");



            PrescriptionVM prescVM = new PrescriptionVM
            {
                Presc_ID = presc.Presc_ID,
                Patient_ID = presc.Patient_ID,
                Doctor_ID = presc.Doctor_ID,
                Medication = presc.Medication,
                Instructions = presc.Instructions,
                Dosage = presc.Dosage,
                DateIssued = presc.DateIssued
            };

            return View(prescVM);
        }

        [HttpPost]
        public ActionResult Edit_Prescription(PrescriptionVM prescVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_Edit_Prescription", con)) // Correct SP name
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Presc_ID", prescVM.Presc_ID);
                            cmd.Parameters.AddWithValue("@Patient_ID", prescVM.Patient_ID);
                            cmd.Parameters.AddWithValue("@Doctor_ID", prescVM.Doctor_ID);
                            cmd.Parameters.AddWithValue("@Medication", prescVM.Medication);
                            cmd.Parameters.AddWithValue("@Instructions", prescVM.Instructions);
                            cmd.Parameters.AddWithValue("@Dosage", prescVM.Dosage);
                            cmd.Parameters.AddWithValue("@DateIssued", prescVM.DateIssued);

                            con.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    TempData["Message"] = "Prescription record updated successfully.";
                    return RedirectToAction("List_Prescription");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
            }

            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls, "Doctor_ID", "Dr_FirstName", prescVM.Doctor_ID);
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_ID", "P_FirstName", prescVM.Patient_ID);

            return View(prescVM);
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


        public ActionResult Delete_Payment(int PaymentId)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                using (conn)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Delete_Payment", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Payment_ID", PaymentId);
                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("List_Payment", "Admin");
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }
            return RedirectToAction("List_Payment", "Admin");
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
                return RedirectToAction("List_Prescription", "Admin");
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }
            return RedirectToAction("List_Payment", "Admin");
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


                        cmd.Parameters.AddWithValue("@Dept_ID", DeptId);


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

            if (Request.HttpMethod == "POST")
            {
                db.ScheduleTbls.Remove(scheduleTbl);
                db.SaveChanges();

                return RedirectToAction("Schedule");
            }

            return View(scheduleTbl);
        }


        public ActionResult Delete_Contact(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsTbl contactUsTbl = db.ContactUsTbls.Find(id);
            if (contactUsTbl == null)
            {
                return HttpNotFound();
            }
            return View(contactUsTbl);
        }
       
        [HttpPost]        
        public ActionResult Delete_Contact(int id)
        {
            ContactUsTbl contactUsTbl = db.ContactUsTbls.Find(id);
            db.ContactUsTbls.Remove(contactUsTbl);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}



