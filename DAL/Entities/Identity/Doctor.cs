using System;
using System.Collections.Generic;

namespace DAL.Entities.Identity
{
    public class Doctor : User
    {
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool WorkAtHome { get; set; }
        public DateTime StartTimeWork { get; set; }
        public DateTime EndTimeWork { get; set; }
        public virtual List<ReserveDoctor> ReserveDoctorForUsers { get; set; }
        public virtual string NurseID { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual List<DocumentsDoctor> DocumentsDoctor { get; set; }
        public virtual Secretary Secretary { get; set; }
    }
}