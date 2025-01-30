using Patient_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace Patient_Management_System.Controllers
{
    public class PatientsTblController : Controller
    {
        // GET: PatientsTbl
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        // This Action method is Display doctors 
        public ActionResult Doctors()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(PatientsTbl patients, HttpPostedFileBase P_Image)
        {
            string str = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;
            SqlConnection conn = new SqlConnection(str);

            if (ModelState.IsValid)
            {
                string imagePath = null;

                // Handle image upload
                if (P_Image != null && P_Image.ContentLength > 0)
                {
                    string uploadPath = Server.MapPath("~/Content/UploadedImages/");


                    string fileName = Path.GetFileNameWithoutExtension(P_Image.FileName);
                    string extension = Path.GetExtension(P_Image.FileName);
                   imagePath = "~/Content/UploadedImages/" + fileName +DateTime.Now.ToString("yyyyMMddHHmmss") + extension;

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
                // Log validation errors
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
                            cmd.Parameters.AddWithValue("@Password", patients.P_Password); // You should ideally hash the password before sending it

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





    }
}
