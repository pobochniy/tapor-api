using System.Data.Common;
using System.Runtime.CompilerServices;
using Dapper;
using Dodo.Data.Factories;
using Microsoft.Extensions.Logging;
using Tapor.DB.Scripts.Issue;
using Tapor.Shared.Dtos;
using Tapor.Shared.Interfaces;

namespace Tapor.DB;

public class IssueSessionRepository : IIssueRepository
{
    private readonly ILogger<IssueRepository> _logger;
    private readonly IDbSessionFactory _sessionFactory;

    public IssueSessionRepository(
        ILogger<IssueRepository> logger,
        IDbSessionFactory sessionFactory)
    {
        _logger = logger;
        _sessionFactory = sessionFactory;
    }

    public async Task<long> Create(IssueDto dto, CancellationToken ct)
    {
        using var session = await _sessionFactory.OpenAsync(ct);
        var issueId = await session.ExecuteScalarAsync<int>(Sql.IssueCreate,
            new
            {
                assignee = dto.Assignee,
                reporter = dto.Reporter,
                summary = dto.Summary,
                description = dto.Description,
                type = dto.Type,
                status = dto.Status,
                priority = dto.Priority,
                size = dto.Size,
                estimatedTime = dto.EstimatedTime,
                createDate = DateTime.UtcNow,
                dueDate = dto.DueDate,
            }, ct: ct);

        _logger.LogInformation("Issue {IssueId} created", issueId);

        return issueId;
    }

    public async IAsyncEnumerable<IssueDto> GetList([EnumeratorCancellation] CancellationToken ct)
    {
        using var session = await _sessionFactory.OpenAsync(ct);

        await foreach (var issue in ((DbConnection) session.Connection)
                       .QueryUnbufferedAsync<IssueDto>("select * from Issue;")
                       .WithCancellation(ct))
        {
            yield return issue;
        }
    }
}















public static partial class CommandExtensions
{
    public static object AsParams(this IssueDto dto)
    {
        return new
        {
            assignee = dto.Assignee,
            reporter = dto.Reporter,
            summary = dto.Summary,
            description = dto.Description,
            type = dto.Type,
            status = dto.Status,
            priority = dto.Priority,
            size = dto.Size,
            estimatedTime = dto.EstimatedTime,
            createDate = DateTime.UtcNow,
            dueDate = dto.DueDate,
        };
    }
}