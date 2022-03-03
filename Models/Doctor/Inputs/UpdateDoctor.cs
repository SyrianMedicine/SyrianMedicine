namespace Models.Doctor.Inputs
{
    public class UpdateDoctor
    {
        public int DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool WorkAtHome { get; set; }
        public DateTime StartTimeWork { get; set; }
        public DateTime EndTimeWork { get; set; }
        public string Location { get; set; }
        public int State { get; set; }
        public string HomeNumber { get; set; }
        public string City { get; set; }
    }
}