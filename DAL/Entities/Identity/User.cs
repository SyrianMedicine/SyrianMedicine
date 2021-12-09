using DAL.Entities.Identity.Enums;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities.Identity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public string State { get; set; }
        public string HomeNumber { get; set; }
        public Gender Gender { get; set; }
    }
}