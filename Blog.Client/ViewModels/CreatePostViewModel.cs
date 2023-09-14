using System.ComponentModel.DataAnnotations;

namespace Blog.Client.ViewModels
{
    public class CreatePostViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Content { get; set; } = string.Empty;
    }
}
