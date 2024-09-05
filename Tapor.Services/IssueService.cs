using Tapor.Shared;
using Tapor.Shared.Dtos;

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

    public IssueDto? Details(long id)
    {
        if (id != 5) return null;
        
        var res = new IssueDto
        {
            Id = 5,
            Reporter = Guid.Empty,
            Status = 1,
            Summary = "Почистить кофе машину",
            Description = "Очень нужная и серьезная задача, необходимо почистить кофе машину",
            DueDate = DateTime.Now,
            EstimatedTime = 1.5m
        };
        
        return res;
    }
}