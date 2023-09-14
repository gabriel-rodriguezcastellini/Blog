namespace Blog.Model
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime DateOfPublish { get; set; }
        public string Author { get; set; } = string.Empty;
        public Status Status { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<RejectionComment> RejectionComments { get; set; } = new List<RejectionComment>();
    }
}