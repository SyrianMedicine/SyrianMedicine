using DAL.Entities;
using DAL.Entities.Identity;

namespace Services.Seed.GenerateData
{
    public class GenerateRatingForUsers
    {
        public static List<Rating> AddRating(List<User> sicks, List<User> doctors, List<User> nurses, List<User> hospitals)
        {
            List<Rating> ratings = new();
            int cnt = 1;
            foreach (var sick in sicks)
            {
                foreach (var doctor in doctors)
                {
                    Rating rate = new()
                    {
                        userid = sick.Id,
                        RatedUserid = doctor.Id,
                        RateValue = (Rating.Rate)(cnt % 5 == 0 ? 5 : cnt % 5)
                    };
                    ratings.Add(rate);
                    cnt++;
                }

                foreach (var nurse in nurses)
                {
                    Rating rate = new()
                    {
                        userid = sick.Id,
                        RatedUserid = nurse.Id,
                        RateValue = (Rating.Rate)(cnt % 5 == 0 ? 5 : cnt % 5)
                    };
                    ratings.Add(rate);
                    cnt++;
                }

                foreach (var hospital in hospitals)
                {
                    Rating rate = new()
                    {
                        userid = sick.Id,
                        RatedUserid = hospital.Id,
                        RateValue = (Rating.Rate)(cnt % 5 == 0 ? 5 : cnt % 5)
                    };
                    ratings.Add(rate);
                    cnt++;
                }
            }
            return ratings;
        }

    }
}