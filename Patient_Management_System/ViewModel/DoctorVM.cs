using Patient_Management_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class DoctorVM
    {
        public int Doctor_ID { get; set; }

        [Required(ErrorMessage = "Please Enter First Name")]
        public string Dr_FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter LastName")]
        public string Dr_LastName { get; set; }


        [Required(ErrorMessage = "Please Enter Email")]
        public string Dr_Email { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string Dr_Password { get; set; }
        public Nullable<System.DateTime> Dr_DOB { get; set; }
        public string Dr_Gender { get; set; }
        public string Dr_Phone { get; set; }
        [Required(ErrorMessage = "Please Enter Qualification")]
        public string Dr_Qualification { get; set; }
        public string Dr_Address { get; set; }
        public string Dr_City { get; set; }
        public string Dr_State { get; set; }
        public int Dr_Pincode { get; set; }
        [Required(ErrorMessage = "Please Choose Image")]
        public string Dr_ImagePath { get; set; }
        public string Dr_Status { get; set; }
        public int Fees { get; set; }
        public int Dept_ID { get; set; }
        public string Dept_Name { get; set; }
        public DepartmentTbl Department { get; set; }
    }
}