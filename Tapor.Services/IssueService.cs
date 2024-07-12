using Tapor.DB;
using Tapor.Shared.Dtos;

namespace Tapor.Services;

public class IssueService
{
    public long Create(IssueDto dto, Guid currentUserId)
    {
        var repository = new IssueRepository();
        var issueId = repository.Create(dto);

        // отправляем уведомления
        var notificationService = new NotificationService();
        var reporter = dto.Reporter.HasValue ? dto.Reporter.Value : currentUserId;
        notificationService.IssueNotify(reporter, false, issueId);
        
        if (dto.Assignee.HasValue)
        {
            notificationService.IssueNotify(dto.Assignee.Value, true, issueId);
        }

        return issueId;
    }
}