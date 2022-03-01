using System.Text.Json;
using DAL.DataContext;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Services.Seed
{
    public class CitySeed
    {
        public static async Task SeedCitiesAsync(StoreContext dbContext)
        {
            if (!await dbContext.Cities.AnyAsync())
            {
                var citiesData = await File.ReadAllTextAsync(@"../Services/Services/Seed/Data/cities.json");
                var cities = JsonSerializer.Deserialize<List<City>>(citiesData);
                foreach (var city in cities)
                {
                    await dbContext.AddAsync(new City { Name = city.Name });
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}