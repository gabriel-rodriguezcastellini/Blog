using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Zemoga.IDP.DbContexts;
using Zemoga.IDP.Services;

namespace Zemoga.IDP;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        try
        {
            DefaultAzureCredential azureCredential = new();

            _ = builder.Services.AddDataProtection().PersistKeysToAzureBlobStorage(new Uri(builder.Configuration["DataProtection:Keys"]), azureCredential)
                .ProtectKeysWithAzureKeyVault(new Uri(builder.Configuration["DataProtection:ProtectionKeyForKeys"]), azureCredential);

            SecretClient secretClient = new(new Uri(builder.Configuration["KeyVault:RootUri"]), azureCredential);
            Azure.Response<KeyVaultSecret> secretResponse = secretClient.GetSecret(builder.Configuration["KeyVault:CertificateName"]);
            X509Certificate2 signingCertificate = new(Convert.FromBase64String(secretResponse.Value.Value), (string)null, X509KeyStorageFlags.MachineKeySet);
            _ = builder.Services.AddRazorPages();
            _ = builder.Services.AddScoped<IPasswordHasher<Entities.User>, PasswordHasher<Entities.User>>();
            _ = builder.Services.AddScoped<ILocalUserService, LocalUserService>();

            _ = builder.Services.AddDbContext<IdentityDbContext>(options =>
            {
                _ = options.UseSqlServer(builder.Configuration.GetConnectionString("ZemogaIdentityDBConnectionString"));
            });

            string migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            _ = builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
                .AddProfileService<LocalUserProfileService>()
                //.AddInMemoryIdentityResources(Config.IdentityResources)
                //.AddInMemoryApiScopes(Config.ApiScopes)
                //.AddInMemoryApiResources(Config.ApiResources)
                //.AddInMemoryClients(Config.Clients)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = options => options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityServerDBConnectionString"),
                        sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                })
                .AddConfigurationStoreCache()
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("IdentityServerDBConnectionString"),
                        sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                })
                .AddSigningCredential(signingCertificate);

            _ = builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            return builder.Build();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        _ = app.UseForwardedHeaders();
        _ = app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            _ = app.UseDeveloperExceptionPage();
        }

        _ = app.UseStaticFiles();
        _ = app.UseRouting();
        _ = app.UseIdentityServer();
        _ = app.UseAuthorization();
        _ = app.MapRazorPages().RequireAuthorization();
        return app;
    }
}
