using Blog.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.API.DbContexts
{
    public class BlogContext : DbContext
    {
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<RejectionComment> RejectionComments { get; set; } = null!;

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Post>().HasData(
                new Post()
                {
                    Id = 1,
                    Title = "Pending approval",
                    AuthorId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Content = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec qu",
                    Status = Blog.Model.Status.PendingApproval,
                    DateOfPublish = new DateTime(2023, 09, 12),
                    Author = "David Flagg"
                },
                new Post()
                {
                    Id = 2,
                    Title = "Published",
                    AuthorId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Content = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec qu",
                    Status = Blog.Model.Status.Published,
                    DateOfPublish = new DateTime(2023, 09, 12),
                    Author = "David Flagg"
                },
                new Post()
                {
                    Id = 3,
                    Title = "Rejected",
                    AuthorId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Content = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec qu",
                    Status = Blog.Model.Status.Rejected,
                    DateOfPublish = new DateTime(2023, 09, 12),
                    Author = "David Flagg"
                });

            _ = modelBuilder.Entity<Comment>().HasData(
                new Comment()
                {
                    Id = 1,
                    Content = "Take a look at my post @someone",
                    PostId = 2,
                    UserId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    User = "David Flagg",
                    DateOfPublish = new DateTime(2023, 09, 12)
                },
                new Comment()
                {
                    Id = 2,
                    Content = "Awesome post!",
                    PostId = 2,
                    UserId = "b7539694-97e7-4dfe-84da-b4256e1ff5c8",
                    User = "Gabriel Rodríguez Castellini",
                    DateOfPublish = new DateTime(2023, 09, 12)
                },
                new Comment()
                {
                    Id = 3,
                    Content = "Rejected by the author",
                    PostId = 3,
                    UserId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    User = "Emma Flagg",
                    DateOfPublish = new DateTime(2023, 09, 12)
                });

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected virtual void OnBeforeSaving()
        {
            foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    // Write creation date
                    case EntityState.Added when entry.Entity is Post:
                        entry.Property(nameof(Post.DateOfPublish)).CurrentValue = DateTime.UtcNow;
                        break;
                    case EntityState.Added when entry.Entity is Comment:
                        entry.Property(nameof(Comment.DateOfPublish)).CurrentValue = DateTime.UtcNow;
                        break;
                    case EntityState.Added when entry.Entity is RejectionComment:
                        entry.Property(nameof(RejectionComment.DateOfPublish)).CurrentValue = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}
