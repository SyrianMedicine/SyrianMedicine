namespace Models.Sick.Inputs
{
    public class UpdateReserveDateWithDoctor
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimeReverse { get; set; }
        public int ReserveState { get; set; } =1;
    }
}