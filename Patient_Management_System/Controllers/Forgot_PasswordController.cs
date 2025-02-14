using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Patient_Management_System.Models;
using System.Configuration;
using Patient_Management_System.ViewModel;

namespace Patient_Management_System.Controllers
{
    public class Forgot_PasswordController : Controller
    {
        private readonly Patient_Management_SystemEntities db = new Patient_Management_SystemEntities();
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString;

        // GET: Forgot_Password
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                string newPassword = GenerateRandomPassword();
                string result = ResetUserPassword(model.Email, newPassword);

                if (result == "Success")
                {
                    // Send new password via email
                    SendEmail(model.Email, "Password Reset"," Please Do Not Reply , Your  new password is: " + newPassword);

                    ViewBag.Message = "A new password has been sent to your email.";
                    return RedirectToAction( "Login","Doctor");
                }
                else
                {
                    ViewBag.Message = "Email not found.";
                }
            }
            return View();
        }

        private string ResetUserPassword(string email, string newPassword)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[sp_Reset_Password]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@NewPassword", newPassword);

                    con.Open();
                    string result = cmd.ExecuteScalar().ToString();
                    con.Close();

                    return result;
                }
            }
        }

        private string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString().Substring(0, 8); // Generates an 8-character random password
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("hemangkanzariya00@gmail.com", "LiveDoc Multispecialist Hospital");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "your-email-password";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com", 
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("hemangkanzariya00@gmail.com", "sknt ivrj otjx hjvd") 
            };


            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
