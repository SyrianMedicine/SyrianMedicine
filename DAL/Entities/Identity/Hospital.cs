using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Entities.Identity.Enums;

namespace DAL.Entities.Identity
{
    public class Hospital
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AboutHospital { get; set; }
        public AccountState AccountState { get; set; }
        public string WebSite { get; set; }
        public virtual List<Department> Departments { get; set; }
        public virtual List<ReserveHospital> ReserveHospitalForUsers { get; set; }
        public virtual List<DocumentsHospital> DocumentsHospital { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}