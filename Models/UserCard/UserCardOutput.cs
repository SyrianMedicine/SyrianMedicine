using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity.Enums;

namespace Models.UserCard
{
    /// <summary>
    /// this used to display in comment and post
    /// </summary>
    public class UserCardOutput
    {
        public String UserName { get; set; }
        public String DisplayName { get; set; }
        public string PictureUrl { get; set; }
        public Gender Gender { get; set; }
        public UserType UserType { get; set; }

    }
}