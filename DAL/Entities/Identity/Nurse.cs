using System.Collections.Generic;

namespace DAL.Entities.Identity
{
    public class Nurse : User
    {
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool WorkAtHome { get; set; }
        public int StartHoursTimeWork { get; set; }
        public int StartMinutesTimeWork { get; set; }
        public int EndHoursTimeWork { get; set; }
        public int EndMinutesTimeWork { get; set; }
        public virtual List<ReserveNurse> ReserveNurseForUsers { get; set; }
        public virtual List<DocumentsNurse> DocumentsNurse { get; set; }
    }
}