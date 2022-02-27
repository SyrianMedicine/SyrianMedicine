using Microsoft.AspNetCore.Http;

namespace Models.Hospital.Inputs
{
    public class RegisterHospital
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Location { get; set; }
        public string AboutHospital { get; set; }
        public string Email { get; set; }
        public string PhoneNumer { get; set; }
        public string HomeNumber { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
        public IFormFile[] Documents { get; set; }
    }
}