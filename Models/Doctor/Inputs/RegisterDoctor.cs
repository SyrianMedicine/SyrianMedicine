using Microsoft.AspNetCore.Http;

namespace Models.Doctor.Inputs
{
    public class RegisterDoctor
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int Gender { get; set; }
        public string AboutMe { get; set; }
        public string Specialization { get; set; }
        public bool WorkAtHome { get; set; }
        public DateTime StartTimeWork { get; set; }
        public DateTime EndTimeWork { get; set; }
        public string Location { get; set; }
        public int State { get; set; }
        public string HomeNumber { get; set; }
        public string City { get; set; }
        public IFormFile[] Files { get; set; }
    }
}