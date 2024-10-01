using FluentMigrator.Runner;

namespace Tapor.Migrator.AppConfig;

public static class Config
{
    /// <summary>
    /// Configure the dependency injection services
    /// </summary>
    public static ServiceProvider CreateServices()
    {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add MySql support to FluentMigrator
                .AddMySql8()
                // Set the connection string
                .WithGlobalConnectionString("server=localhost;port=3309;database=tapordb;user=root;password=1234")
                // Define the assembly containing the migrations
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false);
    }
    
    /// <summary>
    /// Update the database
    /// </summary>
    public static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }
}