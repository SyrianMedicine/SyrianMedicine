using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Rating
    {
        public enum Rate { one = 1, two, three, four, five }
        public int Id { get; set; }
        public string userid { get; set; }
        public virtual User User { get; set; }
        public Rate RateValue { get; set; }
        public string RatedUserid { get; set; }
        public virtual User RatedUser { get; set; }
    }
}