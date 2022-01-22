using System.ComponentModel.DataAnnotations;

namespace DAL.Entities.Identity
{
    public class Secretary
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}