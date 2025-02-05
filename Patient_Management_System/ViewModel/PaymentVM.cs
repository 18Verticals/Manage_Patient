using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class PaymentVM
    {
        public int Patient_ID { get; set; }
        public int Payment_ID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public string Status { get; set; }

        public string Remarks { get; set; }
    }
}