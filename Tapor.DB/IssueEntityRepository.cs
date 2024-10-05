using Microsoft.EntityFrameworkCore;
using Tapor.DB.Entity;
using Tapor.Shared.Dtos;
using Tapor.Shared.Enums;
using Tapor.Shared.Interfaces;

namespace Tapor.DB;

public class IssueEntityRepository : IIssueRepository
{
    private readonly ApplicationContext _db;

    public IssueEntityRepository(ApplicationContext db)
    {
        _db = db;
    }

    public async Task<long> Create(IssueDto dto, CancellationToken ct)
    {
        var issue = new Issue
        {
            Assignee = dto.Assignee,
            Reporter = dto.Reporter!.Value,
            Summary = dto.Summary,
            Description = dto.Description,
            Type = (IssueTypeEnum) dto.Type,
            Status = IssueStatusEnum.todo,
            Priority = (IssuePriorityEnum) dto.Priority,
            Size = (SizeEnum) dto.Size,
            CreateDate = DateTime.UtcNow,
            DueDate = dto.DueDate,
            // EpicLink = dto.EpicLink
        };

        // issue.EstimatedTime = await CalcEstimatedTime(dto.Assignee, dto.Size);

        await _db.Issue.AddAsync(issue, ct);
        await _db.SaveChangesAsync(ct);

        return issue.Id;
    }

    public IAsyncEnumerable<IssueDto> GetList(CancellationToken ct)
    {
        var issues = _db.Issue
            .Select(x => new IssueDto
            {
                Id = x.Id,
                Assignee = x.Assignee,
                EstimatedTime = x.EstimatedTime,
                Description = x.Description,
                DueDate = x.DueDate,
                // EpicLink = x.EpicLink,
                Priority = (int)x.Priority,
                Reporter = x.Reporter,
                Size = (int)x.Size,
                Status = (int)x.Status,
                Summary = x.Summary,
                Type = (int)x.Type
            })
            // .Where(i => i.EpicLink == (epicId ?? i.EpicLink))
            .AsAsyncEnumerable();

        return issues;
    }
    
    
    public async Task Update(IssueDto dto)
    {
        var issue = await _db.Issue.FindAsync(dto.Id);

        issue.Assignee = dto.Assignee;
        issue.Reporter = dto.Reporter!.Value;
        issue.Summary = dto.Summary;
        issue.Description = dto.Description;
        issue.Type = (IssueTypeEnum)dto.Type;
        issue.Status = (IssueStatusEnum)dto.Status;
        issue.Priority = (IssuePriorityEnum)dto.Priority;
        issue.EstimatedTime = dto.EstimatedTime;
        issue.Size = (SizeEnum)dto.Size;
        issue.DueDate = dto.DueDate;
        // issue.EpicLink = dto.EpicLink;

        await _db.SaveChangesAsync();
    }

    public async Task Delete(long id)
    {
        var issue = await _db.Issue.FindAsync(id);
        _db.Issue.Remove(issue);

        await _db.SaveChangesAsync();
    }
    public async Task<IssueDto?> Details(long id)
    {
        var issue = await _db.Issue.FindAsync(id);
        var issueDto = new IssueDto
        {
            Id = issue.Id,
            Assignee = issue.Assignee,
            EstimatedTime = issue.EstimatedTime,
            Description = issue.Description,
            DueDate = issue.DueDate,
            // EpicLink = issue.EpicLink,
            Priority = (int)issue.Priority,
            Reporter = issue.Reporter,
            Size = (int)issue.Size,
            Status = (int)issue.Status,
            Summary = issue.Summary,
            Type = (int)issue.Type
        };

        return issueDto;
    }
}