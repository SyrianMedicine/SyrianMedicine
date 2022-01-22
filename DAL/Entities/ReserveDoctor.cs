using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class ReserveDoctor
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual User User { get; set; }
    }
}