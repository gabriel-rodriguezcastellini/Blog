using Blog.API;
using Blog.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Test
{
    public class ServiceCollectionTests
    {
        [Fact]
        public void RegisterDataServices_Execute_DataServicesAreRegistered()
        {
            // Arrange
            ServiceCollection serviceCollection = new();
            IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { { "ConnectionStrings:BlogDBConnectionString", "AnyValueWillDo" } }).Build();

            // Act
            _ = serviceCollection.RegisterDataServices(configuration);
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetService<IPostRepository>());
            _ = Assert.IsType<PostRepository>(serviceProvider.GetService<IPostRepository>());
        }
    }
}
