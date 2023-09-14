using Blog.Model;
using System.ComponentModel.DataAnnotations;

namespace Blog.Client.ViewModels
{
    public class PostRejectionViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime DateOfPublish { get; set; }
        public string Author { get; set; } = string.Empty;

        [Display(Name = "Rejection Comment")]
        [MaxLength(200)]
        public string? RejectionComment { get; set; }

        public ICollection<RejectionComment> RejectionComments { get; set; } = new List<RejectionComment>();
    }
}
