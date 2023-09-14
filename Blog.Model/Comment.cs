namespace Blog.Model
{
    public class Comment
    {
        public string User { get; set; } = string.Empty;
        public DateTime DateOfPublish { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
