namespace Models.Doctor.Outputs
{
    public class DoctorOutput
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string PictureUrl { get; set; }
        public string Location { get; set; }
        public string State { get; set; }
        public string HomeNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string AboutMe { get; set; }
        public string City { get; set; }
        public string Specialization { get; set; }
        public bool WorkAtHome { get; set; }
        public DateTime StartTimeWork { get; set; }
        public DateTime EndTimeWork { get; set; }
    }
}