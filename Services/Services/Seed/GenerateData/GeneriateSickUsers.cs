using System.Text.Json;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;

namespace Services.Seed.GenerateData
{
    public class GeneriateSickUsers
    {
        private static int counter = 100;
        public static List<User> AddUsers()
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
                    Email = firstName + counter.ToString() + "@gmail.com",
                    UserName = firstName + counter.ToString(),
                    HomeNumber = Faker.Phone.Number(),
                    PhoneNumber = Faker.Phone.Number(),
                    Location = Faker.Address.StreetName(),
                    EmailConfirmed = true,
                    UserType = UserType.Sick,
                    Gender = (Gender)(counter % 2 == 0 ? 2 : counter % 2),
                    State = counter % 2 == 0 ? PersonState.Married : PersonState.Single,
                    City = cities[counter % cities.Count].Name,
                };

                users.Add(user);
            }
            return users;
        }
    }
}