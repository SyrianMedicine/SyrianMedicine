namespace Models.Doctor.Outputs
{
    public class ReserveDoctorOutput
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime TimeReverse { get; set; }
        public string ReserveState { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string HomeNumber { get; set; }
    }
}