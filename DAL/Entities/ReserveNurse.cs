using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class ReserveNurse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int NurseId { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual User User { get; set; }
    }
}