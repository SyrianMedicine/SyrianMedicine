using DAL.Entities.Identity.Enums;

namespace Models.Doctor.Outputs
{
    public class CheckReserve
    {
        public int Id { get; set; }
        public DateTime TimeReverse { get; set; }
        public ReserveState ReserveState { get; set; }
        public string UserId { get; set; }
    }
}