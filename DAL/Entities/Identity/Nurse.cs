using System;
using System.Collections.Generic;

namespace DAL.Entities.Identity
{
    public class Nurse : User
    {
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool WorkAtHome { get; set; }
        public DateTime StartTimeWork { get; set; }
        public DateTime EndTimeWork { get; set; }
        public virtual List<ReserveNurse> ReserveNurseForUsers { get; set; }
        public virtual List<DocumentsNurse> DocumentsNurse { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}