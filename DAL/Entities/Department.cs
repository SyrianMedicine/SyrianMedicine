using System.Collections.Generic;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Bed> Beds { get; set; }
        public virtual List<HospitalDepartment> HospitalsDepartments { get; set; }
    }
}