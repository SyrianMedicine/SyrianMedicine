using Models.Helper;

namespace Models.Post.Input
{
    public class PostQuery : Pagination
    {
        public string SearchString { get; set; } = null;
        public string TagName { get; set; } = null;
    }
}