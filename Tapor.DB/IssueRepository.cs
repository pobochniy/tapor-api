using Microsoft.Extensions.Logging;
using Tapor.Shared;
using Tapor.Shared.Dtos;

namespace Tapor.DB;

public class IssueRepository: IIssueRepository
{
    private readonly ILogger _logger;

    public IssueRepository(ILogger logger)
    {
        _logger = logger;
    }
    
    public long Create(IssueDto dto)
    {
        // создаем подключение к базе
        
        // формируем запрос
        
        // сохраняем в базу, получаем ай созданной записи
        var issueId = (long) 5;
        
        // добавить в логгер запись
        _logger.LogInformation("Issue {IssueId} created", issueId);
        
        return issueId;
    }
        
}