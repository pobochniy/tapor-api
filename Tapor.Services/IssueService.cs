using Tapor.Shared;
using Tapor.Shared.Dtos;
using Tapor.Shared.Interfaces;

namespace Tapor.Services;

public class IssueService
{
    private readonly IIssueRepository _repository;
    private readonly NotificationService _notificationService;

    public IssueService(IIssueRepository repository, 
        NotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }
    
    public async Task<long> Create(IssueDto dto, Guid currentUserId, CancellationToken ct)
    {
        if (!dto.Reporter.HasValue)
        {
            dto.Reporter = currentUserId;
        }
        var issueId = await _repository.Create(dto, ct);

        // отправляем уведомления
        _notificationService.IssueNotify(dto.Reporter.Value, false, issueId);
        
        if (dto.Assignee.HasValue)
        {
            _notificationService.IssueNotify(dto.Assignee.Value, true, issueId);
        }

        return issueId;
    }

    public IAsyncEnumerable<IssueDto> GetList(CancellationToken ct)
    {
        return _repository.GetList(ct);
    }

    public async Task<IssueDto?> Details(long id)
    {
        var res = await _repository.Details(id);
        return res;
    }
}