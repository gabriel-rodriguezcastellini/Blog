using System.ComponentModel.DataAnnotations;

namespace Blog.Client.ViewModels
{
    public class EditPostViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Content { get; set; } = string.Empty;

        public DateTime DateOfPublish { get; set; }
        public string Author { get; set; } = string.Empty;
    }
}
