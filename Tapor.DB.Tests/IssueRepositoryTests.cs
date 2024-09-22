using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Tapor.DB.Session;
using Tapor.Shared.Dtos;
using Tapor.Shared.Options;

namespace Tapor.DB.Tests;

public class IssueRepositoryTests
{
    [Test]
    public async Task CanInsert()
    {
        // Arrange
        var logger = Mock.Of<ILogger<IssueRepository>>();
        var dto = new IssueDto
        {
            Assignee = Guid.NewGuid(),
            Reporter = Guid.NewGuid(),
            Summary = "Краткое описание",
            Description = "Полное описание что надо сделать",
            Type = 1,
            Status = 2,
            Priority = 3,
            Size = 4,
            EstimatedTime = 4.3m,
            DueDate = DateTime.Now.AddDays(2)
        };
        var repo = new IssueSessionRepository(logger, new DbSessionFactory(GetConnStringOptions()));

        // Act
        var issueId = await repo.Create(dto, default);
        
        // Assert
        Assert.That(issueId, Is.GreaterThan(0));
    }
    
    [Test]
    public void CanGetList()
    {
        // Arrange
        var logger = Mock.Of<ILogger<IssueRepository>>();
        var repo = new IssueSessionRepository(logger, new DbSessionFactory(GetConnStringOptions()));

        // Act
        var res = repo.GetList(default).ToBlockingEnumerable();
        var kk = res.ToList();
        // Assert
        Assert.That(res.Count(), Is.GreaterThan(0));
    }

    private IOptions<ConnectionStringOptions> GetConnStringOptions()
    {
        return Options.Create(new ConnectionStringOptions
        {
            AppConnection = "server=localhost;port=3309;database=tapordb;user=root;password=1234"
        });
    }

    // private IConfiguration GetAppConnection()
    // {
    //     var connStrings = new Dictionary<string, string>
    //     {
    //         {"ConnectionStrings:AppConnection", "server=localhost;port=3309;database=tapordb;user=root;password=1234"}
    //     };
    //     var configuration = new ConfigurationBuilder()
    //         .AddInMemoryCollection(connStrings!)
    //         .Build();
    //
    //     return configuration;
    // }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    //Summary = "', '', 0, 0, 0, 0, 0, CURDATE(), CURDATE()); drop table Issue; -- ",
}