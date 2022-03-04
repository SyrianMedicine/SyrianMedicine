namespace Models.Sick.Inputs
{
    public class ReserveDateWithNurse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimeReverse { get; set; }
        public int NurseId { get; set; }
    }
}