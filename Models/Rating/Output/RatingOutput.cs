using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;

namespace Models.Rating.Output
{
    public class RatingOutput
    {
        public class RatingOutputRow
        {
            public DAL.Entities.Rating.Rate StarNumber { get; set; }
            public int Count { get; set; }
        }


        public List<RatingOutputRow> RatingData { get; set; }
        public double Average
        {
            get
            {
                double count = 0.0;
                double sum = 0.0;
                RatingData.ForEach(i =>
                {
                    count += i.Count;
                    sum += i.Count * (int)i.StarNumber;
                });
                return sum / count;
            }
        }
    }
}