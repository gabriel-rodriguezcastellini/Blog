using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.API.Entities
{
    public class RejectionComment
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Content { get; set; }

        [ForeignKey("PostId")]
        public Post? Post { get; set; }
        public int PostId { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string User { get; set; } = string.Empty;

        public DateTime DateOfPublish { get; set; }
    }
}
