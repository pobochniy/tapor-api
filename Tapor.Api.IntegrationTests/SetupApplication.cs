using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Tapor.Api.IntegrationTests.ArrangeEntityBuilders;
using Tapor.DB.Entity;

namespace Tapor.Api.IntegrationTests;

public static class SetupApplicationExt
{
    public static async Task<HttpClient> SetupApplication(this ApiApplicationFactory<Program> factory,
        Action<ApplicationContext>? dbArrange = null, string dbName = "", bool withAuth = true)
    {
        var client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<ApplicationContext>(options =>
                    options
                        .UseInMemoryDatabase("Testing_" + dbName)
                        .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    );

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                if (dbArrange != null) dbArrange(db);
                db.SaveChanges();
            });
        }).CreateClient();

        if (withAuth)
        {
            var usr = new
            {
                Login = UserBuilder.SuperAdminName,
                Password = UserBuilder.SuperAdminPassword
            };
            await client.PostAsJsonAsync("/api/Auth/Login", usr);
        }

        return client;
    }
}