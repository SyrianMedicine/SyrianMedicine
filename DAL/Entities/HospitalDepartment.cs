using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class HospitalDepartment
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        [ForeignKey(nameof(HospitalId))]
        public virtual Hospital Hospital { get; set; }
        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }
    }
}