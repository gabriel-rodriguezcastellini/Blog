using Blog.API.Entities;

namespace Blog.API.Services
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetPublishedPostsAsync();
        Task<Post?> GetPostAsync(int id);
        Task AddCommentForPostAsync(int postId, Comment comment);
        Task<bool> SaveChangesAsync();
        Task<bool> PostExistsAsync(int postId);
        Task<IEnumerable<Post>> GetPostsAsync(string authorId);
        void Update(Post post);
        void AddPost(Post post);
        Task<IEnumerable<Post>> GetPendingPosts();
        Task AddRejectionCommentForPostAsync(int postId, RejectionComment rejectionComment);
        Task<bool> PostStatusIsPublishedAsync(int postId);
    }
}
