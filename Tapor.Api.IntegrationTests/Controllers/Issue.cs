using System.Net.Http.Json;
using NUnit.Framework;
using Tapor.Api.IntegrationTests.Arranges;
using Tapor.Api.IntegrationTests.Asserts;
using Tapor.Shared.Dtos;
using Tapor.Shared.Enums;

namespace Tapor.Api.IntegrationTests.Controllers;

public class Issue
{
    [Test]
    public async Task Details()
    {
        // Arrange
        var admin = Given.Admin();
        var issue = Given.Issue(admin.Id);

        var client = await Given.ApiClient(x =>
        {
            x.Profiles.Add(admin);
            x.Issue.Add(issue);
        });

        // Act
        var response = await client.GetAsync($"/api/Issue/Details?id={issue.Id}");

        // Assert
        await response.ShouldBeSuccessful();
        var result = await response.Content.ReadFromJsonAsync<IssueDto>();
        result.AssertEquals(issue);
    }

    [Test]
    public async Task Create()
    {
        // Arrange
        var admin = Given.Admin();
        var client = await Given.ApiClient(x => x.Profiles.Add(admin));
        var issueDto = ArrangeIssues.TestIssue;

        // Act
        var response = await client.PostAsJsonAsync<IssueDto>("/api/Issue/Create", issueDto);

        // Assert
        await response.ShouldBeSuccessful();
        var result = await response.Content.ReadAsStringAsync();
        var issueId = int.Parse(result);
    }

    [Test]
    public async Task Create_Validation()
    {
        // Arrange
        var admin = Given.Admin();
        var client = await Given.ApiClient(x => x.Profiles.Add(admin));
        var emptyIssueDto = new IssueDto();

        // Act
        var response = await client.PostAsJsonAsync<IssueDto>("/api/Issue/Create", emptyIssueDto);

        // Assert
        await response.ShouldBeValidation(new[] {"Summary"});
    }

    [Test]
    public async Task GetList()
    {
        // Arrange
        var admin = Given.Admin();
        var issue = Given.Issue(admin.Id);
        var issue2 = Given.Issue(admin.Id, 1);
        var issue3 = Given.Issue(admin.Id, 2);

        var client = await Given.ApiClient(x =>
        {
            x.Profiles.Add(admin);
            x.Issue.Add(issue);
            x.Issue.Add(issue2);
            x.Issue.Add(issue3);
        });

        // Act
        var response = await client.GetAsync("/api/Issue/GetList");

        // Assert
        await response.ShouldBeSuccessful();
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<IssueDto>>();
        Assert.True(result.Count() == 3);
        result.Single(x => x.Id == 42).AssertEquals(issue);
    }
}