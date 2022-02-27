namespace Models.Doctor.Inputs
{
    public class UpdateDoctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null;
        public string? LastName { get; set; } = null;
        public string PhoneNumber { get; set; } = null;
        public string AboutMe { get; set; } = null;
        public string Specialization { get; set; } = null;
        public bool WorkAtHome { get; set; }
        public DateTime StartTimeWork { get; set; }
        public DateTime EndTimeWork { get; set; }
        public string Location { get; set; } = null;
        public int State { get; set; } = -1;
        public string HomeNumber { get; set; } = null;
        public string City { get; set; } = null;
    }
}