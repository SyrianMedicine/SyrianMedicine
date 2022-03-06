namespace Models.Sick.Inputs
{
    public class ReserveBedInHospital
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int BedId { get; set; }
    }
}