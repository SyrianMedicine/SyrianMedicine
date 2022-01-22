using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Entities.Identity.Enums;

namespace DAL.Entities.Identity
{
    public class Doctor
    {
        public int Id { get; set; }
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool WorkAtHome { get; set; }
        public DateTime StartTimeWork { get; set; }
        public DateTime EndTimeWork { get; set; }
        public AccountState AccountState { get; set; }
        public virtual List<ReserveDoctor> ReserveDoctorForUsers { get; set; }
        public int? NurseID { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual List<DocumentsDoctor> DocumentsDoctor { get; set; }
        public virtual Secretary Secretary { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}