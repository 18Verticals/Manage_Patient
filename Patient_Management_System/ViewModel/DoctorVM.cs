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
        [Key]
        public int Doctor_ID { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string Dr_FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string Dr_LastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Dr_Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Dr_Password { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> Dr_DOB { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public string Dr_Gender { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(10, ErrorMessage = "Phone number cannot exceed 10 digits.")]
        public string Dr_Phone { get; set; }
        public string Dr_Qualification { get; set; }
        public string Dr_Address { get; set; }
        public string Dr_City { get; set; }
        public string Dr_State { get; set; }
        [Required(ErrorMessage = "Pincode is required.")]
        [Range(100000, 999999, ErrorMessage = "Pincode must be a 6-digit number.")]
        public int Dr_Pincode { get; set; }
        public string Dr_ImagePath { get; set; }
        public string Status { get; set; }
        public string Dr_Status { get; set; }
        [Required(ErrorMessage = "Fees are required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Fees must be a positive number.")]
        public int Fees { get; set; }
        public List<DateTime> Available_Date { get; set; } = new List<DateTime>();
        public int Dept_ID { get; set; }
        public TimeSpan? Start_Time { get; set; }
        public TimeSpan? End_Time { get; set; }
        public string Dept_Name { get; set; }
        public DepartmentTbl Department { get; set; }
        public List<ScheduleTbl> Schedule { get; set; }
        public List<ScheduleVM> Schedules { get; set; } = new List<ScheduleVM>();
        public Dictionary<DateTime, (TimeSpan StartTime, TimeSpan EndTime, string Status)> DateTimeSlots { get; set; }
    = new Dictionary<DateTime, (TimeSpan, TimeSpan, string)>();
        public DateTime? SelectedDate { get; set; }
        public TimeSpan? SelectedStartTime { get; set; }
        public TimeSpan? SelectedEndTime { get; set; }

    }
}