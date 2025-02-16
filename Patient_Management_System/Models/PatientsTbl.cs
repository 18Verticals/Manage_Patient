//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Patient_Management_System.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PatientsTbl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PatientsTbl()
        {
            this.AppointmentTbls = new HashSet<AppointmentTbl>();
            this.LoginInfoes = new HashSet<LoginInfo>();
            this.PaymentTbls = new HashSet<PaymentTbl>();
            this.PrescriptionTbls = new HashSet<PrescriptionTbl>();
        }
    
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
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppointmentTbl> AppointmentTbls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LoginInfo> LoginInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentTbl> PaymentTbls { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrescriptionTbl> PrescriptionTbls { get; set; }
    }
}
