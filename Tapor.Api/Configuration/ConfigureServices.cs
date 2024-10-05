using Dodo.Data.Factories;
using Tapor.DB;
using Tapor.DB.Session;
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
            .AddTransient<IssueService>()
            .AddTransient<IIssueRepository, IssueEntityRepository>()
            .AddSingleton<NotificationService>()
            .AddSingleton<NotificationsRepository>()
            .AddSingleton<IDbSessionFactory, DbSessionFactory>();

        return serviceCollection;
    }
}