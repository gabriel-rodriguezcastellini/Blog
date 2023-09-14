using Blog.Model;

namespace Blog.Client.ViewModels
{
    public class BlogViewModel
    {
        public IEnumerable<Post> Posts { get; private set; } = new List<Post>();

        public BlogViewModel(IEnumerable<Post> posts)
        {
            Posts = posts;
        }
    }
}
