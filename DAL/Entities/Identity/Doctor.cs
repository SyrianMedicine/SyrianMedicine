using System.Collections.Generic;

namespace DAL.Entities.Identity
{
    public class Doctor : User
    {
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool WorkAtHome { get; set; }
        public int StartHoursTimeWork { get; set; }
        public int StartMinutesTimeWork { get; set; }
        public int EndHoursTimeWork { get; set; }
        public int EndMinutesTimeWork { get; set; }
        public virtual List<ReserveDoctor> ReserveDoctorForUsers { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual List<DocumentsDoctor> DocumentsDoctor { get; set; }
    }
}