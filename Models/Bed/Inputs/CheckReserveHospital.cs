using DAL.Entities.Identity.Enums;

namespace Models.Bed.Inputs
{
    public class CheckReserveHospital
    {
        public int Id { get; set; }
        public DateTime TimeReverse { get; set; }
        public ReserveState ReserveState { get; set; }
        public string UserId { get; set; }
    }
}