using Microsoft.EntityFrameworkCore;
using Tapor.DB.Entity;

namespace Tapor.Api.Configuration;

public static class ConfigureDatabase
{
    public static IServiceCollection AddApplicationContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("AppConnection");
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 28));
        serviceCollection.AddDbContext<ApplicationContext>(opt => opt.UseMySql(
            connString,
            serverVersion,
            x => x.MigrationsAssembly("Tapor.DB")
        ));
        
        return serviceCollection;
    }
}