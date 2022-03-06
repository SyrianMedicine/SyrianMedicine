using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class UserConnection
    {
        public int Id { get; set; }
        public string userid { get; set; }

        [ForeignKey(nameof(userid))]
        public User user { get; set; }
        public string ConnectionID { get; set; }
        public string UserAgent { get; set; }
        public DateTime ConnecteDateTime { get; set; }
    }
}