using AutoMapper;
using Blog.API.Controllers;
using Blog.API.Profiles;
using Blog.API.Services;
using Blog.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Blog.Test
{
    public class PostsControllerTests
    {
        private readonly PostsController _postsController;
        private readonly API.Entities.Post _firstPost;
        private readonly API.Entities.Post _firstPendingPost;
        private const string _authorId = "d860efca-22d9-47fd-8249-791ba61b07c7";
        private readonly Mock<IPostRepository> _postRepositoryMock = new();

        public PostsControllerTests()
        {
            _firstPost = new API.Entities.Post
            {
                Id = 1,
                Author = "Gabriel Rodríguez Castellini",
                Content = "Content of post 1",
                DateOfPublish = DateTime.UtcNow,
                Status = Status.Published,
                Title = "Post 1",
                AuthorId = _authorId
            };
            _firstPendingPost = new API.Entities.Post
            {
                Id = 3,
                Author = "Gabriel Rodríguez Castellini",
                Content = "Content of post 3",
                DateOfPublish = DateTime.UtcNow,
                Status = Status.PendingApproval,
                Title = "Post 3",
                AuthorId = _authorId
            };
            _ = _postRepositoryMock.Setup(x => x.GetPublishedPostsAsync())
                .ReturnsAsync(new List<API.Entities.Post>
                {
                    _firstPost,
                    new API.Entities.Post
                    {
                        Id = 2, Author = "Gabriel Rodríguez Castellini", Content = "Content of post 2", DateOfPublish = DateTime.UtcNow, Status = Status.Published, Title = "Post 2"
                    }
                });
            _ = _postRepositoryMock.Setup(x => x.GetPendingPosts())
                .ReturnsAsync(new List<API.Entities.Post>
                {
                    _firstPendingPost,
                    new API.Entities.Post
                    {
                        Id = 4, Author = "Gabriel Rodríguez Castellini", Content = "Content of post 4", DateOfPublish = DateTime.UtcNow, Status = Status.PendingApproval, Title = "Post 4"
                    }
                });
            _ = _postRepositoryMock.Setup(x => x.GetPostsAsync(_authorId))
                .ReturnsAsync(new List<API.Entities.Post>
                {
                    _firstPost,
                    _firstPendingPost
                });
            _ = _postRepositoryMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
            _ = _postRepositoryMock.Setup(x => x.GetPostAsync(It.IsAny<int>())).ReturnsAsync(new API.Entities.Post
            {
                Id = 5,
                Author = "Gabriel Rodríguez Castellini",
                Content = "Content of post 5",
                DateOfPublish = DateTime.UtcNow,
                Status = Status.Created,
                Title = "Post 5",
                AuthorId = _authorId
            });
            _ = _postRepositoryMock.Setup(x => x.AddRejectionCommentForPostAsync(It.IsAny<int>(), It.IsAny<API.Entities.RejectionComment>())).Returns(Task.CompletedTask);
            _postsController = new(new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PostProfile>();
                cfg.AddProfile<RejectionCommentProfile>();
            })), _postRepositoryMock.Object);
        }

        [Fact]
        public async Task GetPosts_GetAction_ReturnsOkObjectResultWithCorrectAmountOfPosts()
        {
            // Arrange

            // Act
            ActionResult<IEnumerable<Post>> result = await _postsController.GetPosts();

            // Assert
            IEnumerable<Post> posts = Assert.IsAssignableFrom<IEnumerable<Post>>(Assert.IsType<OkObjectResult>(Assert.IsType<ActionResult<IEnumerable<Post>>>(result).Result).Value);
            Assert.Equal(2, posts.Count());
            Post firstPost = posts.First();
            Assert.Equal(_firstPost.Id, firstPost.Id);
            Assert.Equal(_firstPost.Author, firstPost.Author);
            Assert.Equal(_firstPost.Content, firstPost.Content);
            Assert.Equal(_firstPost.DateOfPublish, firstPost.DateOfPublish);
            Assert.Equal(_firstPost.Status, firstPost.Status);
            Assert.Equal(_firstPost.Title, firstPost.Title);
        }

        [Fact]
        public async Task GetPendingPosts_GetAction_ReturnsOkObjectResultWithCorrectAmountOfPosts()
        {
            // Arrange            

            // Act
            ActionResult<IEnumerable<Post>> result = await _postsController.GetPendingPosts();

            // Assert
            IEnumerable<Post> posts = Assert.IsAssignableFrom<IEnumerable<Post>>(Assert.IsType<OkObjectResult>(Assert.IsType<ActionResult<IEnumerable<Post>>>(result).Result).Value);
            Assert.Equal(2, posts.Count());
            Post firstPost = posts.First();
            Assert.Equal(_firstPendingPost.Id, firstPost.Id);
            Assert.Equal(_firstPendingPost.Author, firstPost.Author);
            Assert.Equal(_firstPendingPost.Content, firstPost.Content);
            Assert.Equal(_firstPendingPost.DateOfPublish, firstPost.DateOfPublish);
            Assert.Equal(_firstPendingPost.Status, firstPost.Status);
            Assert.Equal(_firstPendingPost.Title, firstPost.Title);
        }

        [Fact]
        public async Task GetMyPosts_GetAction_ReturnsOkObjectResultWithCorrectAmountOfPosts()
        {
            // Arrange
            _postsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                    {
                        new Claim("sub", _authorId)
                    }, "UnitTest"))
                }
            };

            // Act
            ActionResult<IEnumerable<Post>> result = await _postsController.GetMyPosts();

            // Assert
            IEnumerable<Post> posts = Assert.IsAssignableFrom<IEnumerable<Post>>(Assert.IsType<OkObjectResult>(Assert.IsType<ActionResult<IEnumerable<Post>>>(result).Result).Value);
            Assert.Equal(2, posts.Count());
            Post firstPost = posts.First();
            Assert.Equal(_firstPost.Id, firstPost.Id);
            Assert.Equal(_firstPost.Author, firstPost.Author);
            Assert.Equal(_firstPost.Content, firstPost.Content);
            Assert.Equal(_firstPost.DateOfPublish, firstPost.DateOfPublish);
            Assert.Equal(_firstPost.Status, firstPost.Status);
            Assert.Equal(_firstPost.Title, firstPost.Title);
        }

        [Fact]
        public async Task AddPost_PostAction_ReturnsOkResult()
        {
            // Arrange                                                
            _postsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                    {
                        new Claim("sub", new Guid().ToString()),
                        new Claim("given_name", "Gabriel"),
                        new Claim("family_name", "Rodríguez Castellini")
                    }, "UnitTest"))
                }
            };

            // Act
            ActionResult result = await _postsController.AddPost(new PostForCreation("Test post", "Test post content"));

            // Assert
            _ = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdatePost_PatchAction_ReturnsNoContentResult()
        {
            // Arrange

            // Act
            ActionResult result = await _postsController.UpdatePost(_firstPost.Id, new PostForUpdate("Test post for update", "Test content post for update"));

            // Assert
            _ = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Submit_PostAction_ReturnsNoContentResult()
        {
            // Arrange

            // Act
            ActionResult result = await _postsController.Submit(It.IsAny<int>());

            // Assert
            _ = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Approve_PostAction_ReturnsNoContentResult()
        {
            // Arrange
            _ = _postRepositoryMock.Setup(x => x.GetPostAsync(It.IsAny<int>())).ReturnsAsync(_firstPendingPost);

            // Act
            ActionResult result = await _postsController.Approve(It.IsAny<int>());

            // Assert
            _ = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Reject_PostAction_ReturnsNoContentResult()
        {
            // Arrange
            _ = _postRepositoryMock.Setup(x => x.GetPostAsync(It.IsAny<int>())).ReturnsAsync(_firstPendingPost);

            // Act
            ActionResult result = await _postsController.Reject(It.IsAny<int>(), It.IsAny<PostRejection>());

            // Assert
            _ = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Reject_NotPendingApprovalStatus_MustReturnBadRequest()
        {
            // Arrange
            _ = _postRepositoryMock.Setup(x => x.GetPostAsync(It.IsAny<int>())).ReturnsAsync(_firstPost);

            // Act
            ActionResult result = await _postsController.Reject(It.IsAny<int>(), It.IsAny<PostRejection>());

            // Assert
            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            _ = Assert.IsType<string>(badRequestResult.Value);
        }

        [Fact]
        public async Task Approve_NotPendingApprovalStatus_MustReturnBadRequest()
        {
            // Arrange
            _ = _postRepositoryMock.Setup(x => x.GetPostAsync(It.IsAny<int>())).ReturnsAsync(_firstPost);

            // Act
            ActionResult result = await _postsController.Approve(It.IsAny<int>());

            // Assert
            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            _ = Assert.IsType<string>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdatePost_PublishedStatus_MustReturnBadRequest()
        {
            // Arrange
            _ = _postRepositoryMock.Setup(x => x.GetPostAsync(It.IsAny<int>())).ReturnsAsync(_firstPost);

            // Act
            ActionResult result = await _postsController.UpdatePost(_firstPost.Id, new PostForUpdate("Bad request post", "This must return bad request since the post is already published"));

            // Assert            
            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            _ = Assert.IsType<string>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdatePost_PendingApprovalStatus_MustReturnBadRequest()
        {
            // Arrange
            _ = _postRepositoryMock.Setup(x => x.GetPostAsync(It.IsAny<int>())).ReturnsAsync(_firstPendingPost);

            // Act
            ActionResult result = await _postsController.UpdatePost(_firstPost.Id, new PostForUpdate("Bad request post", "This must return bad request since the post is already published"));

            // Assert            
            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            _ = Assert.IsType<string>(badRequestResult.Value);
        }
    }
}
