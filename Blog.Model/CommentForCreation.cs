using System.ComponentModel.DataAnnotations;

namespace Blog.Model
{
    public class CommentForCreation
    {
        [Required]
        [MaxLength(50)]
        public string Content { get; set; }

        public CommentForCreation(string content)
        {
            Content = content;
        }
    }
}
