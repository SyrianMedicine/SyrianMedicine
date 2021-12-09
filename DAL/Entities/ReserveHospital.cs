using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class ReserveHospital
    {
        public string HospitalId { get; set; }
        public string UserId { get; set; }
        public int BedId { get; set; }
        public virtual Hospital Hospital { get; set; }
        public virtual User User { get; set; }
        public virtual Bed Bed { get; set; }
    }
}