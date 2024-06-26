using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen(options =>
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

var app = builder.Build();
// app.UseStaticFiles();
app.MapHealthChecks("healthz");
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // options.SwaggerEndpoint("https://localhost:7011/swagger/v1/swagger.json", "v1");
    // options.RoutePrefix = string.Empty;
    // options.InjectStylesheet("swagger-ui/custom.css");
});
app.MapControllers();

app.Run();