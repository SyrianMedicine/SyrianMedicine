using DAL.Entities.Enums;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;

namespace DAL.Entities
{
    public class ReserveNurse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime TimeReverse { get; set; }
        public ReserveState ReserveState { get; set; }
        public ReserveTypes ReserveType { get; set; }
        public string UserId { get; set; }
        public int NurseId { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual User User { get; set; }
    }
}