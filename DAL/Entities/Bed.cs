using System.Collections.Generic;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Bed
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
        public int DepartmentId { get; set; }
        public int HospitalId { get; set; }
        public virtual Hospital Hospital { get; set; }
        public virtual Department Department { get; set; }
        public virtual List<ReserveHospital> ReserveHospital { get; set; }
    }
}