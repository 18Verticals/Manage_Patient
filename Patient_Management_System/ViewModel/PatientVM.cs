using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class PatientVM
    {
        public int Patient_Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string P_FirstName { get; set; }
        public string P_MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string P_LastName { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public string P_Gender { get; set; }
        public Nullable<System.DateTime> P_DOB { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string P_Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(10, ErrorMessage = "Phone number cannot exceed 10 digits.")]
        public string P_Phone { get; set; }
        public string P_BloodGrp { get; set; }
        public string P_Address { get; set; }
        public string P_City { get; set; }
        public string P_State { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [Range(100000, 999999, ErrorMessage = "Pincode must be a 6-digit number.")]
        public string P_Pincode { get; set; }
        public string P_Message { get; set; }
        public string P_Image { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string P_Password { get; set; }
    }
}