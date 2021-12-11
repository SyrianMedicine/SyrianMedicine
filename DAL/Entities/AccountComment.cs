using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class AccountComment : Comment
    {
        public int OnAccountId { get; set; }
        public virtual User OnAccount { get; set; } 
    }
}