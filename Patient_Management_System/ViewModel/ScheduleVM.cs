using Patient_Management_System.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class ScheduleVM
    {
        public int Schedule_ID { get; set; }
        public int Doctor_ID { get; set; }
        public string Dr_FirstName { get; set; }
        public string Dr_LastName { get; set; }
        public int Dept_ID { get; set; }
        public string Dept_Name { get; set; }
        public TimeSpan? Start_Time { get; set; }
        public TimeSpan? End_Time { get; set; }
        public string Status { get; set; }
        public string Doctor_Name { get; set; }
        public string Department_Name { get; set; }
        public string Avl_Date { get; set; }
        public Nullable<System.DateTime> Available_Date { get; set; }
        public virtual ICollection<AppointmentTbl> AppointmentTbls { get; set; }
        public virtual DepartmentTbl DepartmentTbl { get; set; }
        public virtual DoctorTbl DoctorTbl { get; set; }
    }
}