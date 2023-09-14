using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Zemoga.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("roles", "Your role(s)", new [] {"role"}),
            new IdentityResource("given_name", new[]{"given_name"}),
            new IdentityResource("family_name", new[]{"family_name"})
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("blogapi", "Blog API", new[] {"role", "given_name", "family_name"})
            {
                Scopes =
                {
                    "blogapi.fullaccess",
                    "blogapi.read",
                    "blogapi.write"
                },
                ApiSecrets = {new Secret("apisecret".Sha256())}
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            {
                new ApiScope("blogapi.fullaccess"),
                new ApiScope("blogapi.read"),
                new ApiScope("blogapi.write")
            };

    public static IEnumerable<Client> Clients =>
        new Client[]
            {
                new Client()
                {
                    ClientName = "Blog",
                    ClientId = "blogclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = 120,
                    RedirectUris =
                    {
                        "https://blogclient.azurewebsites.net/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://blogclient.azurewebsites.net/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "given_name",
                        "family_name",
                        "blogapi.read",
                        "blogapi.write"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }
            };
}