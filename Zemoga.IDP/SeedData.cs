using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Serilog;

namespace Zemoga.IDP
{
    public class SeedData
    {
        public static void EnsureSeedData(WebApplication application)
        {
            using IServiceScope scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            EnsureSeedData(scope.ServiceProvider.GetService<ConfigurationDbContext>());
        }

        private static void EnsureSeedData(ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                Log.Debug("Clients being populated");

                foreach (Duende.IdentityServer.Models.Client client in Config.Clients.ToList())
                {
                    _ = context.Clients.Add(client.ToEntity());
                }

                _ = context.SaveChanges();
            }
            else
            {
                Log.Debug("Clients already populated");
            }

            if (!context.IdentityResources.Any())
            {
                Log.Debug("IdentityResources being populated");

                foreach (Duende.IdentityServer.Models.IdentityResource resource in Config.IdentityResources.ToList())
                {
                    _ = context.IdentityResources.Add(resource.ToEntity());
                }

                _ = context.SaveChanges();
            }
            else
            {
                Log.Debug("IdentityResources already populated");
            }

            if (!context.ApiScopes.Any())
            {
                Log.Debug("ApiScopes being populated");

                foreach (Duende.IdentityServer.Models.ApiScope resource in Config.ApiScopes.ToList())
                {
                    _ = context.ApiScopes.Add(resource.ToEntity());
                }

                _ = context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiScopes already populated");
            }

            if (!context.ApiResources.Any())
            {
                Log.Debug("ApiResources being populated");

                foreach (Duende.IdentityServer.Models.ApiResource resource in Config.ApiResources.ToList())
                {
                    _ = context.ApiResources.Add(resource.ToEntity());
                }

                _ = context.SaveChanges();
            }
            else
            {
                Log.Debug("ApiResources already populated");
            }
        }
    }
}
