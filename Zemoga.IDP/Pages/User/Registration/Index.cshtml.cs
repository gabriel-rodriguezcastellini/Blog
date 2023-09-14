using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zemoga.IDP.Entities;
using Zemoga.IDP.Services;

namespace Zemoga.IDP.Pages.User.Registration
{
    [AllowAnonymous]
    [SecurityHeaders]
    public class IndexModel : PageModel
    {
        private readonly ILocalUserService _localUserService;
        private readonly IIdentityServerInteractionService _interactionService;

        public IndexModel(ILocalUserService localUserService, IIdentityServerInteractionService interactionService)
        {
            _localUserService = localUserService ?? throw new ArgumentNullException(nameof(localUserService));
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IActionResult OnGet(string returnUrl)
        {
            BuildModel(returnUrl);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                BuildModel(Input.ReturnUrl);
                return Page();
            }
            Entities.User userToCreate = new()
            {
                UserName = Input.UserName,
                Subject = Guid.NewGuid().ToString(),
                Email = Input.Email,
                Active = false,
                Claims = new List<UserClaim>()
                {
                    new()
                    {
                        Type = JwtClaimTypes.GivenName,
                        Value = Input.GivenName
                    },
                    new()
                    {
                        Type = JwtClaimTypes.FamilyName,
                        Value = Input.FamilyName
                    }
                }
            };
            _localUserService.AddUser(userToCreate, Input.Password);
            _ = await _localUserService.SaveChangesAsync();
            string activationLink = Url.PageLink("/user/activation/index", values: new { securityCode = userToCreate.SecurityCode });
            Console.WriteLine(activationLink);
            return Redirect("~/User/ActivationCodeSent");
        }

        private void BuildModel(string returnUrl)
        {
            Input = new InputModel
            {
                ReturnUrl = returnUrl
            };
        }
    }
}
