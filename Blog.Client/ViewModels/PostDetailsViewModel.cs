using Blog.Model;
using System.ComponentModel.DataAnnotations;

namespace Blog.Client.ViewModels
{
    public class PostDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime DateOfPublish { get; set; }
        public string Author { get; set; } = string.Empty;
        public Status Status { get; set; }

        [Required]
        [Display(Name = "Your Comment *")]
        [MaxLength(200)]
        public string NewComment { get; set; } = string.Empty;

        public ICollection<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
        public ICollection<RejectionCommentViewModel> RejectionComments { get; set; } = new List<RejectionCommentViewModel>();
    }
}
