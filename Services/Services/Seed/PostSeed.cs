using DAL.DataContext;
using Microsoft.EntityFrameworkCore;
using Services.Seed.GenerateData;

namespace Services.Seed
{
    public class PostSeed
    {
        public static async Task SeedPostsAsync(StoreContext dbConteext)
        {
            if (!await dbConteext.Posts.AnyAsync())
            {
                var docotrs = await dbConteext.Doctors.ToListAsync();
                var nurses = await dbConteext.Nurses.ToListAsync();
                var hospitals = await dbConteext.Hospitals.ToListAsync();
                var data = GeneratePosts.AddPosts(docotrs, nurses, hospitals);
                await dbConteext.AddRangeAsync(data);
                await dbConteext.SaveChangesAsync();
            }
        }
    }
}