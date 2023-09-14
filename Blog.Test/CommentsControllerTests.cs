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
    public class CommentsControllerTests
    {
        private readonly CommentsController _controller;

        public CommentsControllerTests()
        {
            Mock<IPostRepository> postRepository = new();
            _ = postRepository.Setup(x => x.PostExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _ = postRepository.Setup(x => x.PostStatusIsPublishedAsync(It.IsAny<int>())).ReturnsAsync(true);
            _ = postRepository.Setup(x => x.AddCommentForPostAsync(It.IsAny<int>(), It.IsAny<API.Entities.Comment>())).Returns(Task.CompletedTask);
            _ = postRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);
            _controller = new(new Mapper(new MapperConfiguration(x => x.AddProfile<CommentProfile>())), postRepository.Object);
        }

        [Fact]
        public async Task CreateComment_PostAction_ReturnsOkResult()
        {
            // Arrange                                                
            _controller.ControllerContext = new ControllerContext()
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
            ActionResult result = await _controller.CreateComment(It.IsAny<int>(), new CommentForCreation("Test comment"));

            // Assert
            _ = Assert.IsType<OkResult>(result);
        }
    }
}
