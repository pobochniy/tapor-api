using NUnit.Framework;
using Tapor.Api.IntegrationTests.Controllers;
using Tapor.DB.Entity;
using Tapor.Shared.Dtos;
using Issue = Tapor.DB.Entity.Issue;

namespace Tapor.Api.IntegrationTests.Asserts;

public static class IssueAssert
{
    public static void AssertEquals(this IssueDto dto, Issue entity, bool withId = true)
    {
        if(withId) Assert.True(dto.Id == entity.Id, "Id");
        Assert.True(dto.Assignee == entity.Assignee, "Assignee");
        Assert.True(dto.Reporter == entity.Reporter, "Reporter");
        Assert.True(dto.Priority == (int)entity.Priority, "Priority");
        Assert.True(dto.Summary == entity.Summary, "Summary");
        Assert.True(dto.Description == entity.Description, "Name");
        Assert.True(dto.DueDate == entity.DueDate, "DueDate");
    }
    public static void AssertEquals(this IssueDto dto, IssueDto entity)
    {
        Assert.True(dto.Id == entity.Id, "Id");
        Assert.True(dto.Assignee == entity.Assignee, "Assignee");
        Assert.True(dto.Reporter == entity.Reporter, "Reporter");
        Assert.True(dto.Priority == entity.Priority, "Priority");
        Assert.True(dto.Summary == entity.Summary, "Name");
        Assert.True(dto.Description == entity.Description, "Name");
        Assert.True(dto.DueDate == entity.DueDate, "DueDate");
    }
}