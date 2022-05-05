using System.Text;
using DAL.Entities;
using DAL.Entities.Identity;

namespace Services.Seed.GenerateData
{
    public class GeneratePosts
    {
        public static List<Post> AddPosts(List<Doctor> doctors, List<Nurse> nurses, List<Hospital> hospitals)
        {
            List<Post> posts = new();
            int cnt = 0;
            foreach (var doctor in doctors)
            {
                Post post = new()
                {
                    Date = DateTime.UtcNow,
                    IsEdited = cnt % 2 == 0,
                    PostTitle = Faker.Lorem.Sentence(),
                    PostText = Faker.Lorem.Sentence() + Faker.Lorem.Sentence(),
                    UserId = doctor.UserId,
                    Type = cnt % 10 == 0 ? Post.PostType.post : Post.PostType.Question
                };
                posts.Add(post);
                cnt++;
            }

            foreach (var nurse in nurses)
            {
                Post post = new()
                {
                    Date = DateTime.UtcNow,
                    IsEdited = cnt % 2 == 0,
                    PostTitle = Faker.Lorem.Sentence(),
                    PostText = Faker.Lorem.Sentence() + Faker.Lorem.Sentence(),
                    UserId = nurse.UserId,
                    Type = cnt % 10 == 0 ? Post.PostType.post : Post.PostType.Question
                };
                posts.Add(post);
                cnt++;
            }

            foreach (var hospital in hospitals)
            {
                Post post = new()
                {
                    Date = DateTime.UtcNow,
                    IsEdited = cnt % 2 == 0,
                    PostText = GenreratePostText(),
                    PostTitle = Faker.Lorem.Sentence(),
                    UserId = hospital.UserId,
                    Type = cnt % 15 == 0 ? Post.PostType.post : Post.PostType.Question
                };
                posts.Add(post);
                cnt++;
            }
            return posts;
        }

        private static string GenreratePostText()
        {
            StringBuilder stringBuilder = new();

            foreach (var item in Faker.Lorem.Sentences(6))
            {
                stringBuilder.Append(item);
            }
            return stringBuilder.ToString();
        }
    }
}