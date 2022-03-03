namespace Models.Hospital.Inputs
{
    public class UpdateHospital
    {
        public int HospitalId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string AboutHospital { get; set; }
        public string PhoneNumer { get; set; }
        public string HomeNumber { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
    }
}