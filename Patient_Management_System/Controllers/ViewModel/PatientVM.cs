using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class PatientVM
    {
        public int Patient_Id { get; set; }
        public string P_FirstName { get; set; }
        public string P_MiddleName { get; set; }
        public string P_LastName { get; set; }
        public string P_Gender { get; set; }
        public Nullable<System.DateTime> P_DOB { get; set; }
        public string P_Email { get; set; }
        public string P_Phone { get; set; }
        public string P_BloodGrp { get; set; }
        public string P_Address { get; set; }
        public string P_City { get; set; }
        public string P_State { get; set; }
        public string P_Pincode { get; set; }
        public string P_Message { get; set; }
        public string P_Image { get; set; }
        public string P_Password { get; set; }
    }
}