using Tapor.Shared.Dtos;
using Tapor.Shared.Enums;

namespace Tapor.Api.IntegrationTests.Arranges;

public static class ArrangeIssues
{
    public static readonly IssueDto TestIssue = new IssueDto
    {
        Id = 42,
        Priority = (int)IssuePriorityEnum.high,
        Summary = "Тестовый эпик",
        Description = "Ребята решили протестировать санни дей на эпик",
        DueDate = new DateTime(2007, 1, 1)
    };
}