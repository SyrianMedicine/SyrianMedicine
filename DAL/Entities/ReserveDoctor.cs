using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class ReserveDoctor
    {
        public string UserId { get; set; }
        public string DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual User User { get; set; }
    }
}