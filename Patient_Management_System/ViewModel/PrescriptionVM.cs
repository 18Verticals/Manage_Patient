using Patient_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class PrescriptionVM
    {
        public int Presc_ID { get; set; }
        public int Patient_ID { get; set; }
        public string P_FirstName { get; set; }
        public int Doctor_ID { get; set; }
        public string Dr_FirstName { get; set; }
        public System.DateTime DateIssued { get; set; }
        public string Medication { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }

        public virtual DoctorTbl DoctorTbl { get; set; }
        public virtual PatientsTbl PatientsTbl { get; set; }
    }
}