using Blog.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.RegisterDataServices(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddOAuth2Introspection(options =>
      {
          options.Authority = "https://zemogaidp.azurewebsites.net";
          options.ClientId = "blogapi";
          options.ClientSecret = "apisecret";
          options.NameClaimType = "given_name";
          options.RoleClaimType = "role";
      });

WebApplication app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();