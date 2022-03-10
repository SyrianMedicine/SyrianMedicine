using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;

namespace Models.Rating.Output
{
    public class RatingOutput
    {
        public class RatingOutputRaw
        {
            public DAL.Entities.Rating.Rate StarNumber { get; set; }
            public int Count { get; set; }
        }

        
        public List<RatingOutputRaw> RatingData { get; set; }
        public double Average
        {
            get
            {
                return RatingData.Average(i => ((int)i.StarNumber) * i.Count);
            }
        }
    }
}