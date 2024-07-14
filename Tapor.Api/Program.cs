using System.Reflection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using Tapor.Api.Configuration;

namespace Tapor.Api;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder
        (string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    

}

public class Startup
{
    public Startup(IConfiguration config)
    {
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                o.Cookie.HttpOnly = true;
                o.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = redirectContext =>
                    {
                        redirectContext.HttpContext.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddControllers();
        services.AddHealthChecks();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Tapor api",
                Description = "Апи для работы с супер проектом, который убийца jira",
                TermsOfService = new Uri("/api/Home/About"),
                Contact = new OpenApiContact
                {
                    Email = "admin@tapor.com",
                    Name = "super admin"
                },
                License = new OpenApiLicense
                {
                    Name = "моя лицензия",
                    Url = new Uri("/api/Home/About")
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        services.AddServices();
    }

    public void Configure(IApplicationBuilder app, 
        IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            // определение маршрутов
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
        
        // app.MapHealthChecks("healthz");
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // options.SwaggerEndpoint("https://localhost:7011/swagger/v1/swagger.json", "v1");
            // options.RoutePrefix = string.Empty;
            // options.InjectStylesheet("swagger-ui/custom.css");
        });

        app.UseAuthentication();
        app.UseAuthorization();
        // app.MapControllers();

        // app.Run();
    }
}