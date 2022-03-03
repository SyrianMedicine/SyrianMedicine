using Microsoft.AspNetCore.Http;

namespace Models.User
{
    public class UploadImage
    {
        public IFormFile imageUrl { get; set; }
    }
}