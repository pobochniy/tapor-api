using Tapor.DB;
using Tapor.Services;
using Tapor.Shared;

namespace Tapor.Api.Configuration;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddTransient<AuthService>()
            .AddTransient<IssueService>()
            .AddTransient<IIssueRepository, IssueRepository>()
            .AddTransient<NotificationService>()
            .AddTransient<NotificationsRepository>();
        
        return serviceCollection;
    }
}