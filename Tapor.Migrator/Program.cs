using Tapor.Migrator.AppConfig;

using (var serviceProvider = Config.CreateServices())
using (var scope = serviceProvider.CreateScope())
{
    // Put the database update into a scope to ensure
    // that all resources will be disposed.
    Config.UpdateDatabase(scope.ServiceProvider);
}

