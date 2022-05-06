namespace Models.Sick.Inputs
{
    public class UpdateReserveDateWithNurse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimeReverse { get; set; }
        public int ReserveType { get; set; } =1;
    }
}