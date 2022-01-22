using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Entities.Identity.Enums;

namespace DAL.Entities.Identity
{
    public class Hospital
    {
        public int Id { get; set; }
        public string AboutHospital { get; set; }
        public AccountState AccountState { get; set; }
        public virtual Department Department { get; set; }
        public virtual List<ReserveHospital> ReserveHospitalForUsers { get; set; }
        public virtual List<DocumentsHospital> DocumentsHospital { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}