using DAL.Entities.Identity.Enums;

namespace Models.Sick.Inputs
{
    public class ReserveDateWithDoctor
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimeReverse { get; set; }
        public int DoctorId { get; set; }
    }
}