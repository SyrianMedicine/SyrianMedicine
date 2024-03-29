using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DateTime Date { get; set; }
        public UserType UserType { get; set; } = UserType.Sick;
        public virtual List<ReserveDoctor> ReserveWithDoctor { get; set; }
        public virtual List<ReserveNurse> ReserveWithNurse { get; set; }
        public virtual List<ReserveHospital> ReserveHospital { get; set; }
        public virtual List<UserConnection> UserConnections { get; set; }
        public virtual List<Follow> Followers { get; set; }
        public virtual List<UserTag> UserTag { get; set; }
        public virtual List<Follow> Following { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Nurse Nurse { get; set; }
        public virtual Secretary Secretary { get; set; }
        public virtual Hospital Hospital { get; set; }
        public virtual List<Rating> UsersRatedMe { get; set; }
        public virtual List<Rating> UsersIRate { get; set; }
        [NotMapped]
        public String fullName{get{return FirstName+" "+LastName;}}
    }
}