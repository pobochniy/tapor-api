using Tapor.Shared.Options;

namespace Tapor.Api.Configuration;

public static class ConfigureOptions
{
    public static IServiceCollection AddOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddOptions<ConnectionStringOptions>()
            .Bind(configuration.GetSection("ConnectionStrings"));

        return serviceCollection;
    }
}