namespace Models.Post.Output
{
    public class MostPostsRated
    {
        public int Id { get; set; }
        public string PostTitle { get; set; }
        public string PostText { get; set; }
        public string MediaUrl { get; set; }
        public String UserName { get; set; }
    }
}