using AutoMapper;
using Blog.API.Services;
using Blog.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace Blog.API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;

        public PostsController(IMapper mapper, IPostRepository postRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _postRepository = postRepository;
        }

        [HttpGet("GetPublishedPosts")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return Ok(_mapper.Map<IEnumerable<Post>>(await _postRepository.GetPublishedPostsAsync()));
        }

        [HttpGet("GetPendingPosts")]
        [Authorize(Roles = "Editor")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPendingPosts()
        {
            return Ok(_mapper.Map<IEnumerable<Post>>(await _postRepository.GetPendingPosts()));
        }

        [HttpGet("{id}", Name = "GetPost")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            Entities.Post? postFromRepo = await _postRepository.GetPostAsync(id);
            return postFromRepo == null ? (ActionResult<Post>)NotFound() : (ActionResult<Post>)Ok(_mapper.Map<Post>(postFromRepo));
        }

        [HttpGet("GetMyPosts")]
        [Authorize(Roles = "Writer")]
        public async Task<ActionResult<IEnumerable<Post>>> GetMyPosts()
        {
            string? authorId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            return authorId == null
                ? throw new Exception("User identifier is missing from token.")
                : (ActionResult<IEnumerable<Post>>)Ok(_mapper.Map<IEnumerable<Post>>(await _postRepository.GetPostsAsync(authorId)));
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<ActionResult> UpdatePost(int id, [FromBody] PostForUpdate postForUpdate)
        {
            Entities.Post? postFromRepo = await _postRepository.GetPostAsync(id);

            if (postFromRepo == null)
            {
                return NotFound();
            }

            if (postFromRepo.Status is Status.PendingApproval or Status.Published)
            {
                return BadRequest($"Cannot edit post with {postFromRepo.Status.GetDisplayName()} status");
            }

            _ = _mapper.Map(postForUpdate, postFromRepo);
            _postRepository.Update(postFromRepo);
            _ = await _postRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost()]
        [Authorize(Roles = "Writer")]
        public async Task<ActionResult> AddPost([FromBody] PostForCreation postForCreation)
        {
            Entities.Post postEntity = _mapper.Map<Entities.Post>(postForCreation);
            postEntity.AuthorId = (User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value) ?? throw new Exception("User identifier is missing from token.");

            postEntity.Author = $"{(User.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value) ?? throw new Exception("User given name is missing from token.")} " +
                $"{(User.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value) ?? throw new Exception("User family name is missing from token.")}";

            _postRepository.AddPost(postEntity);
            _ = await _postRepository.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/submit")]
        [Authorize(Roles = "Writer")]
        public async Task<ActionResult> Submit(int id)
        {
            Entities.Post? postFromRepo = await _postRepository.GetPostAsync(id);

            if (postFromRepo == null)
            {
                return NotFound();
            }

            if (postFromRepo.Status is not Status.Created and not Status.Rejected)
            {
                return BadRequest("Cannot submit the post due to its status");
            }

            postFromRepo.Status = Status.PendingApproval;
            _postRepository.Update(postFromRepo);
            _ = await _postRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Editor")]
        public async Task<ActionResult> Approve(int id)
        {
            Entities.Post? postFromRepo = await _postRepository.GetPostAsync(id);

            if (postFromRepo == null)
            {
                return NotFound();
            }

            if (postFromRepo.Status != Status.PendingApproval)
            {
                return BadRequest("Cannot approve the post due to its status");
            }

            postFromRepo.Status = Status.Published;
            _postRepository.Update(postFromRepo);
            _ = await _postRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Editor")]
        public async Task<ActionResult> Reject(int id, [FromBody] PostRejection postRejection)
        {
            Entities.Post? postFromRepo = await _postRepository.GetPostAsync(id);

            if (postFromRepo == null)
            {
                return NotFound();
            }

            if (postFromRepo.Status != Status.PendingApproval)
            {
                return BadRequest("Cannot reject the post due to its status");
            }

            postFromRepo.Status = Status.Rejected;
            _postRepository.Update(postFromRepo);
            Entities.RejectionComment rejectionComment = _mapper.Map<Entities.RejectionComment>(postRejection);

            if (rejectionComment != null)
            {
                rejectionComment.UserId = (User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value) ?? throw new Exception("User identifier is missing from token.");

                rejectionComment.User = $"{(User.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value) ?? throw new Exception("User given name is missing from token.")} " +
                    $"{(User.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value) ?? throw new Exception("User family name is missing from token.")}";

                await _postRepository.AddRejectionCommentForPostAsync(id, rejectionComment);
            }

            _ = await _postRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
