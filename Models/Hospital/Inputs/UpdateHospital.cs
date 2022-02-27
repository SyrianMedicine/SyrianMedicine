namespace Models.Hospital.Inputs
{
    public class UpdateHospital
    {
        public int Id { get; set; }
        public string Name { get; set; } = null;
        public string Location { get; set; } = null;
        public string AboutHospital { get; set; } = null;
        public string PhoneNumer { get; set; } = null;
        public string HomeNumber { get; set; } = null;
        public string WebSite { get; set; } = null;
        public string City { get; set; } = null;
    }
}