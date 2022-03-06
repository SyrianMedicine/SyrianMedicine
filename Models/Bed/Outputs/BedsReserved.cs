namespace Models.Bed.Outputs
{
    public class BedsReserved
    {
        public int Id { get; set; }
        public string HospitalName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ReserveState { get; set; }
    }
}