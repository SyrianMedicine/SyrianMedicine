using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class HospitalHistory
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string UserId { get; set; }
        public int BedId { get; set; }
        public virtual User User { get; set; }
        public virtual Bed Bed { get; set; }
    }
}