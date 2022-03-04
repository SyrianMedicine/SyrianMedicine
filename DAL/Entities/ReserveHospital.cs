using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;

namespace DAL.Entities
{
    public class ReserveHospital
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public ReserveState ReserveState { get; set; }
        public string UserId { get; set; }
        public int BedId { get; set; }
        public virtual User User { get; set; }
        public virtual Bed Bed { get; set; }
    }
}