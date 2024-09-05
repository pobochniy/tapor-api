using Tapor.DB;
using Tapor.Services;
using Tapor.Shared;
using Tapor.Shared.Interfaces;

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

        return serviceCollection;
    }
}