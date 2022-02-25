using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Entities.Identity.Enums;
using Microsoft.AspNetCore.Identity;

namespace DAL.Entities.Identity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string PictureUrl { get; set; }
        public string Location { get; set; }
        public PersonState State { get; set; }
        public string HomeNumber { get; set; }
        public Gender Gender { get; set; }
        public string City { get; set; }
        public UserType UserType { get; set; } = UserType.Sick;
        public virtual List<ReserveDoctor> ReserveWithDoctor { get; set; }
        public virtual List<ReserveNurse> ReserveWithNurse { get; set; }
        public virtual List<ReserveHospital> ReserveHospital { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual Secretary Secretary { get; set; }
        public virtual Hospital Hospital { get; set; }

    }
}