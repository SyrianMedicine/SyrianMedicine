using DAL.Entities;
using DAL.Entities.Identity;

namespace Services.Seed.GenerateData
{
    public class GenerateRatingForUsers
    {
        public static List<Rating> AddRating(List<User> sicks, List<User> doctors, List<User> nurses, List<User> hospitals)
        {
            var random=new Random();
            List<Rating> ratings = new(); 
            foreach (var sick in sicks)
            {
                foreach (var doctor in doctors)
                {
                    Rating rate = new()
                    {
                        userid = sick.Id,
                        RatedUserid = doctor.Id,
                        RateValue =((Rating.Rate) random.Next(1,5))
                    };
                    ratings.Add(rate);  
                }

                foreach (var nurse in nurses)
                {
                    Rating rate = new()
                    {
                        userid = sick.Id,
                        RatedUserid = nurse.Id,
                        RateValue = ((Rating.Rate) random.Next(1,5))
                    };
                    ratings.Add(rate); 
                }

                foreach (var hospital in hospitals)
                {
                    Rating rate = new()
                    {
                        userid = sick.Id,
                        RatedUserid = hospital.Id,
                        RateValue = ((Rating.Rate) random.Next(1,5))
                    };
                    ratings.Add(rate); 
                }
            }
            return ratings;
        }

    }
}