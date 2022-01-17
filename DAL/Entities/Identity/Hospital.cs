using System.Collections.Generic;

namespace DAL.Entities.Identity
{
    public class Hospital
    {
        public string AboutHospital { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public virtual List<ReserveHospital> ReserveHospitalForUsers { get; set; }
        public virtual List<DocumentsHospital> DocumentsHospital { get; set; }
    }
}