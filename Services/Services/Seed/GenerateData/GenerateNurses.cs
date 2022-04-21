using System.Text.Json;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;

namespace Services.Seed.GenerateData
{
    public class GenerateNurses
    {
        private static string[] specialties = new string[]{
            "Allergy and immunology",
            "Anesthesiology",
            "Dermatology",
            "Emergency",
            "Family",
            "Internal",
            "Medical genetics",
            "Neurology",
            "Nuclear",
            "Ophthalmology",
            "Pathology",
            "Pediatrics",
            "Physical",
            "Preventive",
            "Psychiatry",
            "Radiation oncology",
            "Surgery",
            "Urology"
        };
        private static int counter = 100;
        private static List<User> AddUsers()
        {
            List<User> users = new();

            while ((counter--) > 0)
            {
                var citiesData = File.ReadAllText(@"../Services/Services/Seed/Data/cities.json");
                var cities = JsonSerializer.Deserialize<List<City>>(citiesData);
                string firstName = Faker.Name.First();
                User user = new()
                {
                    FirstName = firstName,
                    LastName = Faker.Name.Last(),
                    Email = "Nurse" + firstName + counter.ToString() + "@gmail.com",
                    UserName = "Nurse" + firstName + counter.ToString(),
                    HomeNumber = Faker.Phone.Number(),
                    PhoneNumber = Faker.Phone.Number(),
                    Location = Faker.Address.StreetName(),
                    EmailConfirmed = true,
                    UserType = UserType.Nurse,
                    Gender = (Gender)(counter % 2 == 0 ? 2 : counter % 2),
                    State = counter % 2 == 0 ? PersonState.Married : PersonState.Single,
                    City = cities[counter % cities.Count].Name,
                };

                users.Add(user);
            }
            return users;
        }

        public static (List<Nurse>, List<User>) AddNurses()
        {
            // var startTime = new DateTime(2021, 10, 10, 10, 30, 0);
            // var endTime = new DateTime(2021, 10, 10, 16, 30, 0);
            var startTime =DateTime.UtcNow;
            var endTime =startTime.AddHours(2);
            
            List<Nurse> nurses = new();
            List<User> users = AddUsers();
            int cnt = 0;
            foreach (var user in users)
            {
                cnt++;
                Nurse nurse = new()
                {
                    AboutMe = Faker.Lorem.Sentence(),
                    AccountState = AccountState.Approved,
                    Specialization = specialties[cnt % specialties.Length],
                    UserId = user.Id,
                    WorkAtHome = cnt % 2 == 0,
                    StartTimeWork = startTime,
                    EndTimeWork = endTime,
                };
                nurses.Add(nurse);
            }

            return (nurses, users);

        }
    }
}