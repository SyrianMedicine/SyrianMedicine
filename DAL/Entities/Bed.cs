using System.Collections.Generic;

namespace DAL.Entities
{
    public class Bed
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public virtual List<ReserveHospital> ReserveHospital { get; set; }
    }
}