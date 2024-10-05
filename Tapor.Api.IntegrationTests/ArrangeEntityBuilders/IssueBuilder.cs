using Tapor.DB.Entity;
using Tapor.Shared.Enums;

namespace Tapor.Api.IntegrationTests.ArrangeEntityBuilders;

public class IssueBuilder
{
    private Issue _issue;

    public IssueBuilder()
    {
        _issue = new Issue
        {
            Id = 42,
            Priority = IssuePriorityEnum.high,
            Summary = "Тестовое ишшью",
            Description = "Ребята решили протестировать санни дей на ишшьюсе",
            DueDate = new DateTime(2007, 1, 1)
        };
    }

    public IssueBuilder WithId(int? issueId)
    {
        if (issueId.HasValue) _issue.Id = issueId.Value;

        return this;
    }

    public IssueBuilder WithAssignee(Guid userId)
    {
        _issue.Assignee = userId;

        return this;
    }

    public IssueBuilder WithReporter(Guid userId)
    {
        _issue.Reporter = userId;

        return this;
    }

    public Issue Please()
    {
        return _issue;
    }
}