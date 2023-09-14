using System.ComponentModel.DataAnnotations;

namespace Blog.Model
{
    public class PostRejection
    {
        [MaxLength(200)]
        public string? RejectionComment { get; set; }

        public PostRejection(string? rejectionComment)
        {
            RejectionComment = rejectionComment;
        }
    }
}
