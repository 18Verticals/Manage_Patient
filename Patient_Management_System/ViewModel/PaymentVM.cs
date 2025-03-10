using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class PaymentVM
    {
        public int Patient_ID { get; set; }
        public string P_FirstName{get; set;}
        public int Payment_ID { get; set; }
        public decimal Amount { get; set; }
        public string CreditCardNumber { get; set; }
        public string Cvv { get; set; }
        public  DateTime Exp_Date { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;       
        public string Remarks { get; set; }
        public virtual PatientVM PatientVM { get; set; }
    }
}