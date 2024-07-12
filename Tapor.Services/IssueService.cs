using Microsoft.Extensions.Logging;
using Tapor.DB;
using Tapor.Shared.Dtos;

namespace Tapor.Services;

public class IssueService
{
    private readonly ILogger _logger;

    public IssueService(ILogger logger)
    {
        _logger = logger;
    }
    
    public long Create(IssueDto dto, Guid currentUserId)
    {
        var repository = new IssueRepository(_logger);
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