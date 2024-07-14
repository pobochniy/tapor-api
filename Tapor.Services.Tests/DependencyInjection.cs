using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tapor.Api;
using Tapor.Api.Controllers;

namespace Tapor.Services.Tests;

public class DependencyInjection
{
    [Test]
    public void CanResolveApplication()
    {
        var host = Host.CreateDefaultBuilder(new string[] { })
            // .ConfigureAppConfiguration(Configs.ConfigurationExtensions.DodoIsWebConfiguration())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .UseDefaultServiceProvider((_, options) =>
            {
                options.ValidateOnBuild = true;
            })
            .Build();

        Assert.Pass();
    }
    
    
    [Test]
    public void VerifyControllers()
    {
        var builder = new WebHostBuilder()
            // .UseEnvironment("Development")
            // .UseConfiguration(new ConfigurationBuilder().DodoIsConfiguration("Development").Build())
            .UseStartup<Startup>();
        
        var testServer = new TestServer(builder);
        var controllersAssembly = typeof(IssueController).Assembly;
        var controllers = controllersAssembly.ExportedTypes.Where(x => typeof(ControllerBase).IsAssignableFrom(x));
        var activator = testServer.Host.Services.GetService<IControllerActivator>();
        var serviceProvider = testServer.Host.Services.GetService<IServiceProvider>();
        var errors = new Dictionary<Type, Exception>();

        foreach (var controllerType in controllers)
        {
            try
            {
                var actionContext = new ActionContext(
                    new DefaultHttpContext
                    {
                        RequestServices = serviceProvider!
                    },
                    new RouteData(),
                    new ControllerActionDescriptor
                    {
                        ControllerTypeInfo = controllerType.GetTypeInfo()
                    });
                activator!.Create(new ControllerContext(actionContext));
            }
            catch (Exception e)
            {
                errors.Add(controllerType, e);
            }
        }

        if (errors.Any())
        {
            Assert.Fail(
                string.Join(
                    Environment.NewLine,
                    errors.Select(x => $"Failed to resolve controller {x.Key.Name} due to {x.Value.ToString()}")));
        }
    }
}