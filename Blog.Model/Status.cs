using System.ComponentModel.DataAnnotations;

namespace Blog.Model
{
    public enum Status
    {
        [Display(Name = "Created")]
        Created,

        [Display(Name = "Pending approval")]
        PendingApproval,

        [Display(Name = "Published")]
        Published,

        [Display(Name = "Rejected")]
        Rejected
    }
}
