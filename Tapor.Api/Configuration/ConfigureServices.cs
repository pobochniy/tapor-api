using Tapor.DB;
using Tapor.Services;
using Tapor.Shared;

namespace Tapor.Api.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<AuthService>()
            .AddSingleton<IssueService>()
            .AddSingleton<IIssueRepository, IssueRepository>()
            .AddSingleton<NotificationService>()
            .AddSingleton<NotificationsRepository>();

        serviceCollection.AddScoped<SystemUser>();

        serviceCollection.AddSingleton(new MySingleton(1));
        serviceCollection.AddSingleton(new MySingleton(2));
        
        return serviceCollection;
    }
}