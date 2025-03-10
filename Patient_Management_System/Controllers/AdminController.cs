using Microsoft.Ajax.Utilities;
using PagedList;
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
using System.Net.Mail;
using System.Numerics;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Patient_Management_System.Controllers
{
    public class AdminController : Controller
    {
        private readonly Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;

        public ActionResult Index()
        {
            DashboardVM dashboard = new DashboardVM();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetDashboardCounts", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dashboard.TotalDoctors = Convert.ToInt32(reader["TotalDoctors"]);
                            dashboard.TotalPatients = Convert.ToInt32(reader["TotalPatients"]);
                            dashboard.TotalDepartmenets = Convert.ToInt32(reader["TotalDepartments"]);
                        }
                    }
                }
            }
            return View(dashboard);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
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

                        int result = Convert.ToInt32(cmd.ExecuteScalar());

                        if (result > 0)
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid email or password.");
                        }
                    }
                }
            }
            return View(adminVM);
        }
        private List<SelectListItem> GetDoctors()
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
            return doctors;
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

        public ActionResult List_Patient(PatientsTbl patients, int? page, string searchQuery)
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

            if (!string.IsNullOrEmpty(searchQuery))
            {
                patientList = patientList
                    .Where(d => d.P_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0
                             || d.P_MiddleName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0
                             || d.P_LastName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(patientList.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Add_Patient()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add_Patient(PatientVM patients, HttpPostedFileBase P_Image)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                return View(patients);
            }

            string imagePath = UploadImage(P_Image);
            if (imagePath == null)
            {
                ViewBag.Error = "Error uploading image.";
                return View(patients);
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
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
                TempData["SuccessMessage"] = "Patient record has been added successfully.";
                return RedirectToAction("List_Patient", "Admin");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    string errorMessage = ex.Message;

                    if (errorMessage.Contains("P_Email"))
                    {
                        ViewBag.Message = "The Email you entered is already associated with another patient. Please use a different email.";
                    }
                }
                else
                {
                    ViewBag.Message = "An error occurred: " + ex.Message;
                }
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }
            return View(patients);
        }

        [HttpGet]
        public ActionResult Edit_Patient(int patientId)
        {
            var patient = db.PatientsTbls.Where(d => d.Patient_Id == patientId).FirstOrDefault();
            if (patient == null)
            {
                TempData["Error"] = "Patient not found.";
                return RedirectToAction("List_Patient");
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
                P_Message = patient.P_Message,
                P_Image = patient.P_Image
            };
            return View(patientVM);
        }

        [HttpPost]
        public ActionResult Edit_Patient(PatientVM patient, HttpPostedFileBase P_Image)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string oldImagePath = "";
                    using (SqlCommand cmd = new SqlCommand("SELECT P_Image FROM PatientsTbl WHERE Patient_Id = @Patient_Id", con))
                    {
                        cmd.Parameters.AddWithValue("@Patient_Id", patient.Patient_Id);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            oldImagePath = result.ToString();
                        }
                    }

                    string imagePath = oldImagePath;

                    if (P_Image != null && P_Image.ContentLength > 0)
                    {
                        string uploadFolder = Server.MapPath("~/Content/UploadedImages/");
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }
                        string fileName = Path.GetFileNameWithoutExtension(P_Image.FileName) + "_" +
                                           DateTime.Now.Ticks + Path.GetExtension(P_Image.FileName);
                        imagePath = "~/Content/UploadedImages/" + fileName;
                        P_Image.SaveAs(Path.Combine(uploadFolder, fileName));
                    }
                    using (SqlCommand cmd = new SqlCommand("sp_Edit_Patient", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Patient_ID", patient.Patient_Id);
                        cmd.Parameters.AddWithValue("@P_FirstName", patient.P_FirstName);
                        cmd.Parameters.AddWithValue("@P_MiddleName", patient.P_MiddleName);
                        cmd.Parameters.AddWithValue("@P_LastName", patient.P_LastName);
                        cmd.Parameters.AddWithValue("@P_Gender", patient.P_Gender);
                        cmd.Parameters.AddWithValue("@P_DOB", patient.P_DOB ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@P_Email", patient.P_Email);
                        cmd.Parameters.AddWithValue("@P_Phone", patient.P_Phone);
                        cmd.Parameters.AddWithValue("@P_BloodGrp", patient.P_BloodGrp);
                        cmd.Parameters.AddWithValue("@P_Address", patient.P_Address);
                        cmd.Parameters.AddWithValue("@P_City", patient.P_City);
                        cmd.Parameters.AddWithValue("@P_State", patient.P_State);
                        cmd.Parameters.AddWithValue("@P_Pincode", patient.P_Pincode);
                        cmd.Parameters.AddWithValue("@P_Message", patient.P_Message);
                        cmd.Parameters.AddWithValue("@P_Image", imagePath);
                        cmd.ExecuteNonQuery();
                    }
                    ViewBag.ImagePath = imagePath;
                }

                TempData["SuccessMessage"] = "Patient details have been updated successfully!";
                return RedirectToAction("List_Patient");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ViewBag.Message = "The Email you entered is already associated with another patient. Please use a different email.";
                }
                else
                {
                    ViewBag.Message = "An error occurred: " + ex.Message;
                }
                return View(patient);
            }
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
            ViewBag.Dept_ID = GetDepartment();
            return View(doctorVM);
            return RedirectToAction("List_Doctor");
        }

        [HttpPost]
        public ActionResult Edit_Doctor(DoctorVM doctor, HttpPostedFileBase Dr_ImagePath)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string oldImagePath = "";

                    using (SqlCommand cmd = new SqlCommand("SELECT Dr_ImagePath FROM DoctorTbl WHERE Doctor_ID = @Doctor_ID", con))
                    {
                        cmd.Parameters.AddWithValue("@Doctor_ID", doctor.Doctor_ID);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            oldImagePath = result.ToString();
                        }
                    }
                    string imagePath = oldImagePath;
                    if (Dr_ImagePath != null && Dr_ImagePath.ContentLength > 0)
                    {
                        string uploadFolder = Server.MapPath("~/Content/UploadedImages/");
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }
                        string fileName = Path.GetFileNameWithoutExtension(Dr_ImagePath.FileName) + "_" +
                                          DateTime.Now.Ticks + Path.GetExtension(Dr_ImagePath.FileName);
                        imagePath = "~/Content/UploadedImages/" + fileName;
                        Dr_ImagePath.SaveAs(Path.Combine(uploadFolder, fileName));

                        if (!string.IsNullOrEmpty(oldImagePath))
                        {
                            string oldFilePath = Server.MapPath(oldImagePath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }
                    }

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
                        cmd.Parameters.AddWithValue("@Dr_ImagePath", imagePath); // Updated image path
                        cmd.Parameters.AddWithValue("@Dr_Status", doctor.Dr_Status ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Fees", doctor.Fees);
                        cmd.Parameters.AddWithValue("@Dept_ID", doctor.Dept_ID);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Doctor details have been updated successfully!";
                return RedirectToAction("List_Doctor");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ViewBag.Message = "The Email you entered is already associated with another doctor. Please use a different email.";
                }
                else
                {
                    ViewBag.Message = "An error occurred: " + ex.Message;
                }
                ViewBag.Dept_ID = GetDepartment();
                return View(doctor);
            }
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
                        cmd.Parameters.AddWithValue("@Patient_Id", patientId);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Patient Record  has been Deleted successfully.";
                return RedirectToAction("List_Patient", "Admin");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }
            return RedirectToAction("List_Patient", "Admin");
        }

        public ActionResult List_Doctor(DoctorVM doctorVM, int? page, string searchQuery)
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
                        Doctor_ID = Convert.ToInt32(reader["Doctor_ID"] ?? 0),
                        Dr_FirstName = reader["Dr_FirstName"]?.ToString() ?? "",
                        Dr_LastName = reader["Dr_LastName"]?.ToString() ?? "",
                        Dept_ID = Convert.ToInt32(reader["Dept_ID"] ?? 0),
                        Dept_Name = reader["Dept_Name"]?.ToString() ?? "",
                        Dr_Email = reader["Dr_Email"]?.ToString() ?? "",
                        Dr_Password = reader["Dr_Password"]?.ToString() ?? "",
                        Dr_DOB = reader["Dr_DOB"] is DBNull ? DateTime.MinValue : Convert.ToDateTime(reader["Dr_DOB"]),
                        Dr_Gender = reader["Dr_Gender"]?.ToString() ?? "",
                        Dr_Phone = reader["Dr_Phone"]?.ToString() ?? "",
                        Dr_Qualification = reader["Dr_Qualification"]?.ToString() ?? "",
                        Dr_Address = reader["Dr_Address"]?.ToString() ?? "",
                        Dr_City = reader["Dr_City"]?.ToString() ?? "",
                        Dr_State = reader["Dr_State"]?.ToString() ?? "",
                        Dr_Pincode = Convert.ToInt32(reader["Dr_Pincode"] ?? 0),
                        Dr_ImagePath = reader["Dr_ImagePath"]?.ToString() ?? "",
                        Fees = Convert.ToInt32(reader["Fees"] ?? 0),
                        Dr_Status = reader["Dr_Status"]?.ToString() ?? ""
                    };
                    doctorList.Add(doctor);
                }
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                doctorList = doctorList
               .Where(d => d.Dr_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0
                        || d.Dr_LastName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
               .ToList();
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(doctorList.ToPagedList(pageNumber, pageSize));

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
            ViewBag.Dept_ID = GetDepartment();
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
                    ViewBag.Message = "Error uploading image.";
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
                    TempData["SuccessMessage"] = "Doctor Record has been added successfully!";
                    return RedirectToAction("List_Doctor", "Admin");
                }
                catch (SqlException ex)
                {

                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        ViewBag.Message = "The Email you entered is already associated with another doctor. Please use a different email.";
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
            ViewBag.Dept_ID = GetDepartment();
            return View(doctorVM);
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
        public ActionResult List_Schedule(ScheduleVM scheduleVM, int? page, string searchQuery)
        {
            List<ScheduleVM> ScheduleList = new List<ScheduleVM>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_Get_Schedule", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ScheduleVM schedule = new ScheduleVM
                    {
                        Schedule_ID = reader["Schedule_ID"] != DBNull.Value ? Convert.ToInt32(reader["Schedule_ID"]) : 0,
                        Doctor_ID = reader["Doctor_ID"] != DBNull.Value ? Convert.ToInt32(reader["Doctor_ID"]) : 0,
                        Dept_ID = reader["Dept_ID"] != DBNull.Value ? Convert.ToInt32(reader["Dept_ID"]) : 0,
                        Dr_FirstName = reader["Dr_FirstName"].ToString(),
                        Dr_LastName = reader["Dr_LastName"].ToString(),
                        Dept_Name = reader["Dept_Name"].ToString(),
                        Start_Time = reader["Start_Time"] != DBNull.Value ? (TimeSpan)reader["Start_Time"] : TimeSpan.Zero,
                        End_Time = reader["End_Time"] != DBNull.Value ? (TimeSpan)reader["End_Time"] : TimeSpan.Zero,
                        Available_Date = reader["Available_Date"] != DBNull.Value ? DateTime.Parse(reader["Available_Date"].ToString()) : (DateTime?)null,
                        Status = reader["Status"].ToString(),
                    };
                    ScheduleList.Add(schedule);
                }
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                ScheduleList = ScheduleList
                    .Where(d => d.Dr_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Where(d => d.Dr_LastName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(ScheduleList.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult Add_Schedule()
        {
            ViewBag.Doctor_ID = GetDoctors();
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
            return View(new ScheduleVM());
        }
        [HttpPost]
        public ActionResult Add_Schedule(ScheduleVM scheduleVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Doctor_ID = GetDoctors();
                ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
                return View(scheduleVM);
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Add_Schedule", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Doctor_ID", scheduleVM.Doctor_ID);
                        cmd.Parameters.AddWithValue("@Dept_ID", scheduleVM.Dept_ID);
                        cmd.Parameters.AddWithValue("@Start_Time", scheduleVM.Start_Time);
                        cmd.Parameters.AddWithValue("@End_Time", scheduleVM.End_Time);
                        cmd.Parameters.AddWithValue("@Status", scheduleVM.Status);
                        cmd.Parameters.AddWithValue("@Available_Date", scheduleVM.Available_Date);
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Schedule added successfully!";
                return RedirectToAction("List_Schedule", "Admin");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
            }
            return View(scheduleVM);
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

            ViewBag.Dept_ID = new SelectList(GetDepartment(), "Value", "Text", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(GetDoctors(), "Value", "Text", aptVM.Doctor_ID);
            ViewBag.TimeSlots = new SelectList(GetTimeSlots(), "Value", "Text", aptVM.Apt_Time);

            return View(aptVM);
        }

        [HttpPost]
        public ActionResult Edit_Appointment(AppointmentVM aptVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Dept_ID = new SelectList(GetDepartment(), "Value", "Text", aptVM.Dept_ID);
                ViewBag.Doctor_ID = new SelectList(GetDoctors(), "Value", "Text", aptVM.Doctor_ID);

                ViewBag.TimeSlots = new SelectList(GetTimeSlots(), "Value", "Text", aptVM.Apt_Time);

                return View(aptVM);
            }
            try
            {
                if (!int.TryParse(aptVM.Doctor_Name, out int doctorId) ||
                   !int.TryParse(aptVM.Department_Name, out int deptId))
                {
                    TempData["Error"] = "Invalid doctor or department selection.";
                    return View(aptVM);
                }
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Edit_Appointment", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Appointment_ID", aptVM.Appointment_ID);
                        cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);
                        cmd.Parameters.AddWithValue("@Dept_ID", deptId);
                        cmd.Parameters.AddWithValue("@Apt_Date", aptVM.Apt_Date);
                        cmd.Parameters.AddWithValue("@Apt_Time", aptVM.Apt_Time);
                        cmd.Parameters.AddWithValue("@Description", aptVM.Description);
                        cmd.Parameters.AddWithValue("@Phone", aptVM.Phone);
                        cmd.Parameters.AddWithValue("@Diseases", aptVM.Diseases);
                        SqlParameter patientEmailParam = new SqlParameter("@PatientEmail", SqlDbType.NVarChar, 100)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(patientEmailParam);

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

                        SqlParameter returnValue = new SqlParameter
                        {
                            Direction = ParameterDirection.ReturnValue
                        };
                        cmd.Parameters.Add(returnValue);

                        con.Open();
                        cmd.ExecuteNonQuery();

                        int result = (int)returnValue.Value;

                        if (result == 1)
                        {
                            string patientEmail = patientEmailParam.Value?.ToString();
                            string patientName = patientNameParam.Value?.ToString() ?? "Patient";
                            string doctorName = doctorNameParam.Value?.ToString() ?? "Doctor";
                            SendEmailUpdateNotification(patientEmail, patientName, doctorName, aptVM);

                            TempData["SuccessMessage"] = "Appointment updated successfully!";
                            return RedirectToAction("List_Appointment");
                        }
                        else if (result == 0)
                        {
                            ViewBag.Message = "This time slot is already booked.";
                        }
                        else if (result == -1)
                        {
                            ViewBag.Message = "No patient exists with this phone number.";
                        }
                        else if (result == -2)
                        {
                            ViewBag.Message = "Doctor is not available ";

                        }
                        else if (result == -3)
                        {
                            ViewBag.Message = "This Time Slot Not available,Choose another Slot ";
                        }
                        else
                        {
                            ViewBag.Message = "An unexpected error occurred.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            ViewBag.Dept_ID = new SelectList(GetDepartment(), "Value", "Text", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(GetDoctors(), "Value", "Text", aptVM.Doctor_ID);
            ViewBag.TimeSlots = new SelectList(GetTimeSlots(), "Value", "Text", aptVM.Apt_Time);

            return View(aptVM);
        }


        [HttpGet]
        public ActionResult Edit_Prescription(int PrescId)
        {
            var presc = db.PrescriptionTbls.Where(d => d.Presc_ID == PrescId).FirstOrDefault();
            if (presc == null)
            {
                return HttpNotFound();
            }
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
            ViewBag.Patient_ID = new SelectList(GetPatients(), "Value", "Text", prescVM.Patient_ID);
            ViewBag.Doctor_ID = new SelectList(GetDoctors(), "Value", "Text", prescVM.Doctor_ID);
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
                    TempData["SuccessMessage"] = "Prescription details have been updated successfully.";
                    return RedirectToAction("List_Prescription");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "An error occurred: " + ex.Message;
            }
            return View(prescVM);
        }

        [HttpGet]
        public ActionResult Edit_Schedule(int scheduleId)
        {
            if (scheduleId == 0)
            {
                TempData["Error"] = "Invalid schedule ID.";
                return RedirectToAction("List_Doctor");
            }

            var schedule = db.ScheduleTbls.FirstOrDefault(d => d.Schedule_ID == scheduleId);
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
                Available_Date = schedule.Available_Date?.Date,
                Status = schedule.Status
            };

            ViewBag.Dept_ID = new SelectList(GetDepartment(), "Value", "Text", scheduleVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(GetDoctors(), "Value", "Text", scheduleVM.Doctor_ID);

            return View(scheduleVM);
        }


        [HttpPost]
        public ActionResult Edit_Schedule(ScheduleVM scheduleVM)
        {
            try
            {
                ViewBag.Dept_ID = new SelectList(GetDepartment(), "Value", "Text", scheduleVM.Dept_ID);
                ViewBag.Doctor_ID = new SelectList(GetDoctors(), "Value", "Text", scheduleVM.Doctor_ID);

                if (!int.TryParse(scheduleVM.Doctor_Name, out int doctorId) ||
                    !int.TryParse(scheduleVM.Department_Name, out int deptId))
                {
                    TempData["Error"] = "Invalid doctor or department selection.";
                    return View(scheduleVM);
                }



                var affectedPatients = new List<(string Email, string PatientName)>();

                using (var con = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand("sp_Edit_Schedule", con) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.AddWithValue("@Schedule_ID", scheduleVM.Schedule_ID);
                    cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);
                    cmd.Parameters.AddWithValue("@Dept_ID", deptId);
                    cmd.Parameters.AddWithValue("@Start_Time", scheduleVM.Start_Time);
                    cmd.Parameters.AddWithValue("@End_Time", scheduleVM.End_Time);
                    cmd.Parameters.AddWithValue("@Available_Date", scheduleVM.Available_Date);
                    cmd.Parameters.AddWithValue("@Status", scheduleVM.Status);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            affectedPatients.Add((reader["PatientEmail"].ToString(), reader["PatientName"].ToString()));
                }

                if (affectedPatients.Any())
                {
                    string doctorName = GetDoctorName(doctorId);
                    string formattedDate = scheduleVM.Available_Date?.ToString("dd-MM-yyyy") ?? "N/A";
                    string formattedTime = DateTime.Today.Add(scheduleVM.Start_Time ?? TimeSpan.Zero).ToString("hh:mm tt");

                    affectedPatients.ForEach(patient =>
                        SendEmailCancellationNotification(patient.Email, patient.PatientName, doctorName, formattedDate, formattedTime));

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
                TempData["Error"] = "Error: " + ex.Message;
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

        
        public ActionResult List_Appointment(AppointmentVM aptVM, int? page, string searchQuery)
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
                        Dr_LastName = reader["Dr_LastName"].ToString(),
                        Patient_ID = reader["Patient_ID"] != DBNull.Value ? Convert.ToInt32(reader["Patient_ID"]) : (int?)null,
                        P_FirstName = reader["P_FirstName"].ToString(),
                        P_MiddleName = reader["P_MiddleName"].ToString(),
                        Dept_ID = reader["Dept_ID"] != DBNull.Value ? Convert.ToInt32(reader["Dept_ID"]) : (int?)null,
                        Dept_Name = reader["Dept_Name"].ToString(),
                        Apt_Date = reader["Apt_Date"] != DBNull.Value ? Convert.ToDateTime(reader["Apt_Date"]) : DateTime.MinValue,
                        Apt_Time = reader["Apt_Time"] != DBNull.Value ? (TimeSpan?)reader["Apt_Time"] : null,
                        Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                        Diseases = reader["Diseases"] != DBNull.Value ? reader["Diseases"].ToString() : string.Empty,
                        Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : string.Empty,
                    };
                    aptList.Add(apt);
                }
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                aptList = aptList
                    .Where(d =>
                        (d.P_FirstName != null && d.P_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (d.Dept_Name != null && d.Dept_Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (d.Dr_FirstName != null && d.Dr_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(aptList.ToPagedList(pageNumber, pageSize));
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
                    Credentials = new NetworkCredential("hemangkanzariya00@gmail.com", "lqri ukod qdsl qyfx"),
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

        [HttpGet]
        public ActionResult Add_Appointment()
        {
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name");
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls.Select(d => new {
                Doctor_ID = d.Doctor_ID,
                FullName = d.Dr_FirstName + " " + (d.Dr_LastName ?? "")
            }), "Doctor_ID", "FullName");
            ViewBag.TimeSlots = GetTimeSlots();
            return View();
        }

        [HttpPost]
        public ActionResult Add_Appointment(AppointmentVM aptVM)
        {
            if (!ModelState.IsValid)
            {

                ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
                ViewBag.Doctor_ID = new SelectList(db.DoctorTbls.Select(d => new {
                    Doctor_ID = d.Doctor_ID,
                    FullName = d.Dr_FirstName + " " + (d.Dr_LastName ?? "")
                }), "Doctor_ID", "FullName", aptVM.Doctor_ID);
                ViewBag.TimeSlots = GetTimeSlots();
                return View(aptVM);
            }

            if (aptVM.Dept_ID == null || aptVM.Dept_ID == 0)
            {
                ModelState.AddModelError("Dept_ID", "Department is required.");
                ViewBag.Message = "Please select a department.";


                ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
                ViewBag.Doctor_ID = new SelectList(db.DoctorTbls.Select(d => new {
                    Doctor_ID = d.Doctor_ID,
                    FullName = d.Dr_FirstName + " " + (d.Dr_LastName ?? "")
                }), "Doctor_ID", "FullName", aptVM.Doctor_ID);
                ViewBag.TimeSlots = GetTimeSlots();

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
                    string patientEmail = emailParam.Value?.ToString();
                    string patientName = patientNameParam.Value?.ToString() ?? "Patient";
                    string doctorName = doctorNameParam.Value?.ToString() ?? "Doctor";

                    if (result == 1)
                    {
                        SendEmailNotification(patientEmail, patientName, doctorName, aptVM);
                        TempData["SuccessMessage"] = "Appointment booked successfully!";
                        return RedirectToAction("List_Appointment");
                    }
                    else if (result == 0)
                    {
                        ViewBag.Message = "This time slot is already booked!";
                    }

                    else if (result == -2)
                    {
                        ViewBag.Message = "You have already booked an appointment with this doctor on this date.";
                    }
                    else if (result == -1)
                    {
                        ViewBag.Message = "No patient exists with this phone number.";
                    }
                    else if (result == -3)
                    {
                        ViewBag.Message = "No Avaible This Date ";
                    }
                    else if (result == -4)
                    {
                        ViewBag.Message = "This Time Slot Not available,Choose another Slot ";
                    }
                    else if (result == -5)
                    {
                        ViewBag.Message = "Doctor is not available ";
                    }
                    else
                    {
                        ViewBag.Message = "This time slot is already booked!.";
                    }
                }
            }
            ViewBag.Dept_ID = new SelectList(db.DepartmentTbls, "Dept_ID", "Dept_Name", aptVM.Dept_ID);
            ViewBag.Doctor_ID = new SelectList(db.DoctorTbls.Select(d => new {
                Doctor_ID = d.Doctor_ID,
                FullName = d.Dr_FirstName + " " + (d.Dr_LastName ?? "")
            }), "Doctor_ID", "FullName", aptVM.Doctor_ID);
            ViewBag.TimeSlots = GetTimeSlots();

            return View(aptVM);
        }




        public JsonResult GetDoctorsBySpecialty(int deptId)
        {
            var doctors = db.DoctorTbls.Where(d => d.Dept_ID == deptId)
                                       .Select(d => new SelectListItem
                                       {
                                           Value = d.Doctor_ID.ToString(),
                                           Text = d.Dr_FirstName + " " + d.Dr_LastName
                                       }).ToList();
            return Json(doctors, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetAvailableSlots(int doctorId, string date)
        {
            List<string> availableSlots = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAvailableSlots", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);
                    cmd.Parameters.AddWithValue("@Apt_Date", DateTime.Parse(date));

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            availableSlots.Add(dr["AvailableSlot"].ToString());
                        }
                    }
                }
            }
            return Json(availableSlots, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvailableDates(int doctorId)
        {
            List<string> availableDates = new List<string>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAvailableDates", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            availableDates.Add(Convert.ToDateTime(dr["AvailableDate"]).ToString("yyyy-MM-dd"));
                        }
                    }
                }
            }
            return Json(availableDates, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentByDoctor(int doctorId)
        {
            var doctor = db.DoctorTbls.Where(d => d.Doctor_ID == doctorId)
                                      .Select(d => new { Dept_ID = d.Dept_ID })
                                      .FirstOrDefault();
            return Json(doctor, JsonRequestBehavior.AllowGet);
        }

        private void SendEmailUpdateNotification(string email, string patientName, string doctorName, AppointmentVM aptVM)
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
                     <p>Your appointment has been updated with the following details:</p>
                     <ul>
                         <li><strong>Doctor:</strong> Dr. {doctorName}</li>
                         <li><strong>Date:</strong> {aptVM.Apt_Date:dd-MM-yyyy}</li>
                         <li><strong>Time:</strong> {aptVM.Apt_Time}</li>
                         
                     </ul>
                     <p>If you have any questions, please contact us.</p>
                     <p>Thank you!</p>
                     <p>Best Regards,<br/>LiveDoc Multispecialist Hospital</p>
                     <p>Any Query? Please Contact Us: 70465 90890</p>
                 </body>
                 </html>";

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("hemangkanzariya00@gmail.com"),
                    Subject = "Appointment Update Information",
                    Body = emailBody,

                    IsBodyHtml = true
                };
                mail.To.Add(email);
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("hemangkanzariya00@gmail.com","lqri ukod qdsl qyfx"),
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

        public ActionResult List_Prescription(PrescriptionVM prescVM, int? page, string searchQuery)

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
                        Dr_LastName = reader["Dr_LastName"].ToString(),
                        Patient_ID = Convert.ToInt32(reader["Patient_ID"]),
                        P_FirstName = reader["P_FirstName"].ToString(),
                        P_MiddleName = reader["P_MiddleName"].ToString(),
                        DateIssued = Convert.ToDateTime(reader["DateIssued"]),
                        Medication = reader["Medication"].ToString(),
                        Dosage = reader["Dosage"].ToString(),
                        Instructions = reader["Dosage"].ToString(),
                    };
                    PrescList.Add(presc);
                }
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                PrescList = PrescList
                    .Where(d =>
                        (d.P_FirstName != null && d.P_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (d.Dr_FirstName != null && d.Dr_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }


            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(PrescList.ToPagedList(pageNumber, pageSize));
        }

        // GET: Add Prescription
        [HttpGet]
        public ActionResult Add_Prescription(int? Doctor_ID)
        {

            List<SelectListItem> patientList = new List<SelectListItem>();

            if (Doctor_ID.HasValue)
            {
                patientList = db.AppointmentTbls
                    .Where(a => a.Doctor_ID == Doctor_ID.Value)
                    .Select(a => new SelectListItem
                    {
                        Value = a.Patient_ID.ToString(),
                        Text = a.PatientsTbl.P_FirstName + " " + (a.PatientsTbl.P_MiddleName ?? "") + " " + a.PatientsTbl.P_LastName
                    })
                    .Distinct()
                    .ToList();
            }
            ViewBag.Doctor_ID = GetDoctors();
            ViewBag.Patient_ID = GetPatients();
            return View();
        }

        [HttpPost]
        public ActionResult Add_Prescription(PrescriptionVM prescVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("sp_Add_Prescription", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Doctor_ID", prescVM.Doctor_ID);
                            cmd.Parameters.AddWithValue("@Patient_ID", prescVM.Patient_ID);
                            cmd.Parameters.AddWithValue("@DateIssued", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Medication", prescVM.Medication);
                            cmd.Parameters.AddWithValue("@Dosage", prescVM.Dosage);
                            cmd.Parameters.AddWithValue("@Instructions", prescVM.Instructions);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    TempData["SuccessMessage"] = "Prescription added successfully.";
                    return RedirectToAction("List_Prescription", "Admin");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error: " + ex.Message;
                }
            }

            ViewBag.Doctor_ID = GetDoctors();
            ViewBag.Patient_ID = GetPatients();

            return View(prescVM);
        }




        [HttpGet]
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
                                Value = reader["Value"].ToString(),
                                Text = reader["Text"].ToString().Trim()
                            });
                        }
                    }
                }
            }
            return Json(patients, JsonRequestBehavior.AllowGet);
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
                TempData["SuccessMessage"] = "Prescription details has been deleted successfully.";
                return RedirectToAction("List_Prescription", "Admin");
            }
            catch (Exception ex)
            {

                ViewBag.Error = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }


            return RedirectToAction("List_Payment", "Admin");
        }

        public ActionResult List_Department(DepartmentVM departmentVM, int? page, string searchQuery)
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

            if (!string.IsNullOrEmpty(searchQuery))
            {
                departmentList = departmentList
                    .Where(d => d.Dept_Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(departmentList.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult List_Contact(ContactVM ContactVM, int? page, string searchQuery)
        {
            List<ContactVM> ContactList = new List<ContactVM>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_Get_ContactUs", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ContactVM payment = new ContactVM
                    {
                        Feedback_Id = Convert.ToInt32(reader["Feedback_Id"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Message = reader["Message"].ToString(),
                        Phone = reader["Phone"].ToString(),
                    };
                    ContactList.Add(payment);
                }
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                ContactList = ContactList
                    .Where(d => d.Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(ContactList.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult List_Payment(PaymentVM paymentVM, int? page, string searchQuery)
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
                        P_FirstName = reader["P_FirstName"].ToString(),
                        Amount = Convert.ToInt32(reader["Amount"]),
                        PaymentMethod = reader["PaymentMethod"].ToString(),
                        PaymentDate = Convert.ToDateTime(reader["PaymentDate"]),
                        Status = reader["Status"].ToString(),
                        Remarks = reader["Remarks"].ToString(),
                    };
                    PaymentList.Add(payment);
                }
            }
            if (!string.IsNullOrEmpty(searchQuery))
            {
                PaymentList = PaymentList
                    .Where(d => d.P_FirstName.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(PaymentList.ToPagedList(pageNumber, pageSize));
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
                    TempData["SuccessMessage"] = "Department Record has been added successfully.";
                    return RedirectToAction("List_Department", "Admin");
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627 || ex.Number == 2601)
                    {
                        ViewBag.Message = "The Department name is already Exist. Please Enter a different name..";
                    }
                    else
                    {
                        ViewBag.Message = "An error occurred: " + ex.Message;
                    }
                    System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
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
        public ActionResult Edit_Department(int deptId)
        {
            var department = db.DepartmentTbls.Where(d => d.Dept_ID == deptId).FirstOrDefault();
            if (department == null)
            {
                TempData["Error"] = "Department not found.";
                return RedirectToAction("List_Department");
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
                TempData["SuccessMessage"] = "Department details have been updated successfully.";
                return RedirectToAction("List_Department");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
                return View(departmentVM);
            }
        }

        public ActionResult List_Payment(PaymentVM paymentVM)
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
                        P_FirstName = reader["P_FirstName"].ToString(),
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
            TempData["SuccessMessage"] = "Payment details have been Added successfully.";
            return RedirectToAction("List_Payment");
        }

        [HttpGet]
        public ActionResult Add_Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add_Contact(string name, string email, string message, string phone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Add_Contact", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Message", message);
                        cmd.Parameters.AddWithValue("@Phone", phone);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Contact details have been Added successfully.";
                return RedirectToAction("List_Contact");

            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }

            return View();
        }

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
                TempData["SuccessMessage"] = "Contact Details have been  updated Successfully!";

                return RedirectToAction("List_Contact", "Admin");
            }
            return View(contactUsTbl);
        }

        [HttpGet]
        public ActionResult Edit_Payment(int paymentId)
        {
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_Id", "P_FirstName");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Edit_Payment", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Payment_ID", paymentId);

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
                    TempData["Message"] = "Payment details have been updated successfully.";
                    return RedirectToAction("List_Payments");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
            }
            ViewBag.Patient_ID = new SelectList(db.PatientsTbls, "Patient_ID", "P_FirstName", paymentVM.Patient_ID);

            return View(paymentVM);
        }
        public ActionResult Delete_Doctor(int doctorId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Delete_Doctor", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Doctor_ID", doctorId);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Doctor record has been deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the doctor: " + ex.Message;
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
                TempData["SuccessMessage"] = "Payment record has been deleted successfully.";
                return RedirectToAction("List_Payment", "Admin");
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
            try
            {
                string patientEmail = "", patientName = "", doctorName = "", appointmentDate = "", appointmentTime = "";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Delete_Appointment", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Appointment_ID", aptId);
                        cmd.Parameters.Add(new SqlParameter("@PatientEmail", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });
                        cmd.Parameters.Add(new SqlParameter("@PatientName", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });
                        cmd.Parameters.Add(new SqlParameter("@DoctorName", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });
                        cmd.Parameters.Add(new SqlParameter("@AppointmentDate", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output });
                        cmd.Parameters.Add(new SqlParameter("@AppointmentTime", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output });
                        cmd.ExecuteNonQuery();


                        patientEmail = cmd.Parameters["@PatientEmail"].Value?.ToString() ?? "";
                        patientName = cmd.Parameters["@PatientName"].Value?.ToString() ?? "";
                        doctorName = cmd.Parameters["@DoctorName"].Value?.ToString() ?? "";
                        appointmentDate = cmd.Parameters["@AppointmentDate"].Value?.ToString() ?? "";
                        appointmentTime = cmd.Parameters["@AppointmentTime"].Value?.ToString() ?? "";
                    }
                }

                if (!string.IsNullOrEmpty(patientEmail))
                {
                    SendEmailCancellationNotification(patientEmail, patientName, doctorName, appointmentDate, appointmentTime);
                }

                TempData["SuccessMessage"] = "Appointment has been deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the appointment: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex);
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
                TempData["SuccessMessage"] = "Department record has been deleted successfully.";
                return RedirectToAction("List_Department", "Admin");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }

            return RedirectToAction("List_Department", "Admin");
        }

        public ActionResult Delete_Contact(int id)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                using (conn)
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_Delete_Contact", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Feedback_Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Contact record has been deleted successfully.";
                return RedirectToAction("List_Contact", "Admin");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while deleting the doctor: " + ex.Message;
                System.Diagnostics.Debug.WriteLine("Database error: " + ex.Message);
            }

            return RedirectToAction("List_Contact", "Admin");
        }

    }
}
