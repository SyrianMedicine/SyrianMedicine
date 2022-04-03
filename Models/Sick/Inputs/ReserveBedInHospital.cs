namespace Models.Sick.Inputs
{
    public class ReserveBedInHospital
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int DepartmentId { get; set; }
        public int HospitalId { get; set; }
    }
}