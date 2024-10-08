using Tapor.Shared;
using Tapor.Shared.Dtos;

namespace Tapor.Services;

public class IssueService
{
    private readonly IIssueRepository _repository;
    private readonly NotificationService _notificationService;
    private readonly SystemUser _user;

    public IssueService(IIssueRepository repository, 
        NotificationService notificationService,
        SystemUser user)
    {
        _repository = repository;
        _notificationService = notificationService;
        _user = user;
    }
    
    public long Create(IssueDto dto)
    {
        if (!dto.Reporter.HasValue)
        {
            dto.Reporter = _user._userId;
        }
        var issueId = _repository.Create(dto);

        // отправляем уведомления
        _notificationService.IssueNotify(dto.Reporter.Value, false, issueId);
        
        if (dto.Assignee.HasValue)
        {
            _notificationService.IssueNotify(dto.Assignee.Value, true, issueId);
        }

        return issueId;
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