using Patient_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class ScheduleVM
    {
        public int Schedule_ID { get; set; }
        public int Doctor_ID { get; set; }
        public int Dept_ID { get; set; }
        public string Available_Days { get; set; }
        public System.TimeSpan Start_Time { get; set; }
        public System.TimeSpan End_Time { get; set; }
        public string Status { get; set; }
        public virtual DepartmentTbl DepartmentTbl { get; set; }
        public virtual DoctorTbl DoctorTbl { get; set; }
    }
}