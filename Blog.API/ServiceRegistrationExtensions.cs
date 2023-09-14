using Blog.API.DbContexts;
using Blog.API.Services;
using Microsoft.EntityFrameworkCore;

namespace Blog.API
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection RegisterDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddDbContext<BlogContext>(optionsAction =>
            {
                _ = optionsAction.UseSqlServer(configuration["ConnectionStrings:BlogDBConnectionString"]);
            });

            _ = services.AddScoped<IPostRepository, PostRepository>();
            return services;
        }
    }
}
