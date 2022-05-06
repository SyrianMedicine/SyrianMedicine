using DAL.Entities.Identity.Enums;

namespace Models.Doctor.Outputs
{
    public class CheckReserve
    {
        public DateTime TimeReverse { get; set; }
        public ReserveState ReserveState { get; set; }
        public string UserName { get; set; }
    }
}