using System.ComponentModel.DataAnnotations;

namespace Blog.Model
{
    public class PostForCreation
    {
        public PostForCreation(string title, string content)
        {
            Title = title;
            Content = content;
        }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [MaxLength(200)]
        public string Content { get; set; }
    }
}
