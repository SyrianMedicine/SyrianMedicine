using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class ReserveNurse
    {
        public string UserId { get; set; }
        public string NurseId { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual User User { get; set; }
    }
}