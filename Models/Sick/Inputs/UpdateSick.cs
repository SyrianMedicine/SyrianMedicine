namespace Models.Sick.Inputs
{
    public class UpdateSick
    {
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;
        public string PhoneNumber { get; set; } = null;
        public string HomeNumber { get; set; } = null;
        public int Gender { get; set; } = -1;
        public string Location { get; set; } = null;
        public int State { get; set; } = -1;
        public string City { get; set; } = null;
    }
}