using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patient_Management_System.ViewModel
{
    public class ContactVM
    {
        public int Feedback_Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Phone { get; set; }
    }
}