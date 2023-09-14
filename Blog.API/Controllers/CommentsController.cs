using AutoMapper;
using Blog.API.Services;
using Blog.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [Route("api/posts/{postId}/comments")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;

        public CommentsController(IMapper mapper, IPostRepository postRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        }

        [HttpPost()]
        public async Task<ActionResult> CreateComment(int postId, [FromBody] CommentForCreation commentForCreation)
        {
            if (!await _postRepository.PostExistsAsync(postId))
            {
                return NotFound();
            }

            if (!await _postRepository.PostStatusIsPublishedAsync(postId))
            {
                return BadRequest("Unable to add comment to a non-published post");
            }

            Entities.Comment commentEntity = _mapper.Map<Entities.Comment>(commentForCreation);
            commentEntity.UserId = (User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value) ?? throw new Exception("User identifier is missing from token.");

            commentEntity.User = $"{(User.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value) ?? throw new Exception("User given name is missing from token.")} " +
                $"{(User.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value) ?? throw new Exception("User family name is missing from token.")}";

            await _postRepository.AddCommentForPostAsync(postId, commentEntity);
            _ = await _postRepository.SaveChangesAsync();
            return Ok();
        }
    }
}
