using Blog.Model;
using System.ComponentModel.DataAnnotations;

namespace Blog.API.Entities
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Content { get; set; } = string.Empty;

        public DateTime DateOfPublish { get; set; }

        [Required]
        [MaxLength(50)]
        public string AuthorId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Author { get; set; } = string.Empty;

        public Status Status { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<RejectionComment> RejectionComments { get; set; } = new List<RejectionComment>();
    }
}
