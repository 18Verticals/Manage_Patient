using Patient_Management_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class AppointmentVM
    {
        public int Appointment_ID { get; set; }
        public Nullable<int> Doctor_ID { get; set; }    
        public string Dr_FirstName { get; set; }
        public Nullable<int> Patient_ID { get; set; }
        public string P_FirstName { get; set; }
        public Nullable<int> Dept_ID { get; set; }
        public string Dept_Name { get; set; }
        public Nullable<System.DateTime> Apt_Date { get; set; }
        public Nullable<System.TimeSpan> Apt_Time { get; set; }
        public string Description { get; set; }
        public string Diseases { get; set; }
        public string Email { get; set; }
        public virtual DepartmentTbl DepartmentTbl { get; set; }
        public virtual DoctorTbl DoctorTbl { get; set; }
        public virtual PatientsTbl PatientsTbl { get; set; } 
    }
}