using Blog.API.DbContexts;
using Blog.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.API.Services
{
    public class PostRepository : IPostRepository
    {
        private readonly BlogContext _context;

        public PostRepository(BlogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddComment(Comment comment)
        {
            _ = _context.Comments.Add(comment);
        }

        public async Task AddCommentForPostAsync(int postId, Comment comment)
        {
            (await GetPostAsync(postId))?.Comments.Add(comment);
        }

        public void AddPost(Post post)
        {
            _ = _context.Posts.Add(post);
        }

        public async Task AddRejectionCommentForPostAsync(int postId, RejectionComment rejectionComment)
        {
            (await GetPostAsync(postId))?.RejectionComments.Add(rejectionComment);
        }

        public async Task<IEnumerable<Post>> GetPendingPosts()
        {
            return await _context.Posts.Where(x => x.Status == Model.Status.PendingApproval).AsNoTracking().ToListAsync();
        }

        public async Task<Post?> GetPostAsync(int id)
        {
            return await _context.Posts.Include(x => x.Comments).Include(x => x.RejectionComments).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetPostsAsync(string authorId)
        {
            return await _context.Posts.Where(x => x.AuthorId == authorId).OrderByDescending(x => x.DateOfPublish).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
        {
            return await _context.Posts.Where(x => x.Status == Model.Status.Published).OrderByDescending(x => x.DateOfPublish).ToListAsync();
        }

        public async Task<bool> PostExistsAsync(int postId)
        {
            return await _context.Posts.AnyAsync(c => c.Id == postId);
        }

        public async Task<bool> PostStatusIsPublishedAsync(int postId)
        {
            return await _context.Posts.AnyAsync(x => x.Id == postId && x.Status == Model.Status.Published);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void Update(Post post)
        {
            _ = _context.Update(post);
        }
    }
}
