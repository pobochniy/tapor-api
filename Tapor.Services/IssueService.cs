using Microsoft.Extensions.Logging;
using Tapor.DB;
using Tapor.Shared;
using Tapor.Shared.Dtos;

namespace Tapor.Services;

public class IssueService
{
    private readonly IIssueRepository _repository;

    public IssueService(IIssueRepository repository)
    {
        _repository = repository;
    }
    
    public long Create(IssueDto dto, Guid currentUserId)
    {
        if (!dto.Reporter.HasValue)
        {
            dto.Reporter = currentUserId;
        }
        var issueId = _repository.Create(dto);

        // отправляем уведомления
        var notificationService = new NotificationService();
        notificationService.IssueNotify(dto.Reporter.Value, false, issueId);
        
        if (dto.Assignee.HasValue)
        {
            notificationService.IssueNotify(dto.Assignee.Value, true, issueId);
        }

        return issueId;
    }
}