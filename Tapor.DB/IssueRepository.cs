using Microsoft.Extensions.Logging;
using Tapor.Shared;
using Tapor.Shared.Dtos;

namespace Tapor.DB;

public class IssueRepository: IIssueRepository
{
    private readonly ILogger<IssueRepository> _logger;

    public IssueRepository(ILogger<IssueRepository> logger)
    {
        _logger = logger;
    }
    
    public long Create(IssueDto dto)
    {
        // создаем подключение к базе
        
        // формируем запрос
        
        // сохраняем в базу, получаем ай созданной записи
        var issueId = (long) Random.Shared.Next();
        
        // добавить в логгер запись
        _logger.LogInformation("Issue {IssueId} created", issueId);
        
        return issueId;
    }
        
}