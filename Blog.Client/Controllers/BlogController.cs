using Blog.Client.Extensions;
using Blog.Client.ViewModels;
using Blog.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Text;
using System.Text.Json;

namespace Blog.Client.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IHttpClientFactory httpClientFactory, ILogger<BlogController> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region all roles
        public async Task<IActionResult> Index()
        {
            await LogIdentityInformation();
            HttpResponseMessage response = await _httpClientFactory.CreateClient("APIClient").SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/posts/getpublishedposts"), HttpCompletionOption.ResponseHeadersRead);
            _ = response.EnsureSuccessStatusCode();
            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            return View(new BlogViewModel(await JsonSerializer.DeserializeAsync<List<Post>>(responseStream) ?? new List<Post>()));
        }

        public async Task<IActionResult> Details(int id)
        {
            HttpResponseMessage response = await _httpClientFactory.CreateClient("APIClient").SendAsync(new(HttpMethod.Get, $"/api/posts/{id}"), HttpCompletionOption.ResponseHeadersRead);
            _ = response.EnsureSuccessStatusCode();
            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            Post? deserializedPost = await JsonSerializer.DeserializeAsync<Post>(responseStream) ?? throw new Exception("Deserialized post must not be null.");
            return View(new PostDetailsViewModel()
            {
                Id = deserializedPost.Id,
                Author = deserializedPost.Author,
                Content = deserializedPost.Content,
                DateOfPublish = deserializedPost.DateOfPublish,
                Title = deserializedPost.Title,
                Status = deserializedPost.Status,
                Comments = deserializedPost.Comments.Select(x => new CommentViewModel
                {
                    Content = x.Content,
                    DateOfCreation = x.DateOfPublish,
                    User = x.User,
                }).ToList(),
                RejectionComments = deserializedPost.RejectionComments.Select(x => new RejectionCommentViewModel
                {
                    Content = x.Content,
                    DateOfCreation = x.DateOfPublish,
                    User = x.User
                }).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(PostDetailsViewModel postDetailsViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _ = (await _httpClientFactory.CreateClient("APIClient").SendAsync(new HttpRequestMessage(HttpMethod.Post, $"/api/posts/{postDetailsViewModel.Id}/comments")
            {
                Content = new StringContent(JsonSerializer.Serialize(new CommentForCreation(postDetailsViewModel.NewComment)), Encoding.UTF8, "application/json")
            }, HttpCompletionOption.ResponseHeadersRead)).EnsureSuccessStatusCode();
            return RedirectToAction("Details");
        }
        #endregion

        #region Writer
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> MyPosts()
        {
            HttpResponseMessage response = await _httpClientFactory.CreateClient("APIClient").SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/posts/getmyposts"), HttpCompletionOption.ResponseHeadersRead);
            _ = response.EnsureSuccessStatusCode();
            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            return View(new BlogViewModel(await JsonSerializer.DeserializeAsync<List<Post>>(responseStream) ?? new List<Post>()));
        }

        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditPost(int id)
        {
            HttpResponseMessage response = await _httpClientFactory.CreateClient("APIClient").SendAsync(new(HttpMethod.Get, $"/api/posts/{id}"), HttpCompletionOption.ResponseHeadersRead);
            _ = response.EnsureSuccessStatusCode();
            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            Post? deserializedPost = await JsonSerializer.DeserializeAsync<Post>(responseStream) ?? throw new Exception("Deserialized post must not be null.");
            if (deserializedPost.Status is Status.Published or Status.PendingApproval)
            {
                ModelState.AddModelError("", $"Unable to edit the post because it's in {deserializedPost.Status.GetDisplayName()} status.");
                return View();
            }
            return View(new EditPostViewModel()
            {
                Id = deserializedPost.Id,
                Content = deserializedPost.Content,
                Title = deserializedPost.Title,
                Author = deserializedPost.Author,
                DateOfPublish = deserializedPost.DateOfPublish
            });
        }

        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> SubmitPost(int id)
        {
            HttpResponseMessage response = await _httpClientFactory.CreateClient("APIClient").SendAsync(new(HttpMethod.Post, $"/api/posts/{id}/submit"), HttpCompletionOption.ResponseHeadersRead);
            _ = response.EnsureSuccessStatusCode();
            return RedirectToAction("MyPosts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditPost(EditPostViewModel editPostViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _ = (await _httpClientFactory.CreateClient("APIClient").SendAsync(new HttpRequestMessage(HttpMethod.Patch, $"/api/posts/{editPostViewModel.Id}")
            {
                Content = new StringContent(JsonSerializer.Serialize(new PostForUpdate(editPostViewModel.Title, editPostViewModel.Content)), Encoding.UTF8, "application/json")
            }, HttpCompletionOption.ResponseHeadersRead)).EnsureSuccessStatusCode();
            return RedirectToAction("MyPosts");
        }

        [Authorize(Roles = "Writer")]
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreatePost(CreatePostViewModel createPostViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _ = (await _httpClientFactory.CreateClient("APIClient").SendAsync(new HttpRequestMessage(HttpMethod.Post, $"/api/posts")
            {
                Content = new StringContent(JsonSerializer.Serialize(new PostForCreation(createPostViewModel.Title, createPostViewModel.Content)), Encoding.UTF8, "application/json")
            }, HttpCompletionOption.ResponseHeadersRead)).EnsureSuccessStatusCode();
            return RedirectToAction("MyPosts");
        }
        #endregion

        #region Editor
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> PendingPosts()
        {
            HttpResponseMessage response = await _httpClientFactory.CreateClient("APIClient").SendAsync(new HttpRequestMessage(HttpMethod.Get, "/api/posts/getpendingposts"), HttpCompletionOption.ResponseHeadersRead);
            _ = response.EnsureSuccessStatusCode();
            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            return View(new BlogViewModel(await JsonSerializer.DeserializeAsync<List<Post>>(responseStream) ?? new List<Post>()));
        }

        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> ApprovePost(int id)
        {
            HttpResponseMessage response = await _httpClientFactory.CreateClient("APIClient").SendAsync(new(HttpMethod.Post, $"/api/posts/{id}/approve"), HttpCompletionOption.ResponseHeadersRead);
            _ = response.EnsureSuccessStatusCode();
            return RedirectToAction("PendingPosts");
        }

        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> RejectPost(int id)
        {
            HttpResponseMessage response = await _httpClientFactory.CreateClient("APIClient").SendAsync(new(HttpMethod.Get, $"/api/posts/{id}"), HttpCompletionOption.ResponseHeadersRead);
            _ = response.EnsureSuccessStatusCode();
            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            Post? deserializedPost = await JsonSerializer.DeserializeAsync<Post>(responseStream) ?? throw new Exception("Deserialized post must not be null.");
            return View(new PostRejectionViewModel()
            {
                Id = deserializedPost.Id,
                Author = deserializedPost.Author,
                Content = deserializedPost.Content,
                DateOfPublish = deserializedPost.DateOfPublish,
                Title = deserializedPost.Title,
                RejectionComments = deserializedPost.RejectionComments.Select(x => new RejectionComment
                {
                    Content = x.Content,
                    DateOfPublish = x.DateOfPublish,
                    User = x.User
                }).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> RejectPost(PostRejectionViewModel postRejectionViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _ = (await _httpClientFactory.CreateClient("APIClient").SendAsync(new HttpRequestMessage(HttpMethod.Post, $"/api/posts/{postRejectionViewModel.Id}/reject")
            {
                Content = new StringContent(JsonSerializer.Serialize(new PostRejection(postRejectionViewModel.RejectionComment)), Encoding.UTF8, "application/json")
            }, HttpCompletionOption.ResponseHeadersRead)).EnsureSuccessStatusCode();
            return RedirectToAction("PendingPosts");
        }
        #endregion

        private async Task LogIdentityInformation()
        {
            StringBuilder userClaimsStringBuilder = new();
            foreach (System.Security.Claims.Claim claim in User.Claims)
            {
                _ = userClaimsStringBuilder.AppendLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
            _logger.LogInformation($"Identity token & user claims: \n{await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken)} \n{userClaimsStringBuilder}");
            _logger.LogInformation($"Access token: \n{await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)}");
            _logger.LogInformation($"Refresh token: \n{await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken)}");
        }
    }
}
