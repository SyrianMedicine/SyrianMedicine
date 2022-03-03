namespace Models.Admin.Inputs
{
    public class UpdateAdmin
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string HomeNumber { get; set; }
        public int Gender { get; set; }
        public string Location { get; set; }
        public int State { get; set; }
        public string City { get; set; }
    }
}