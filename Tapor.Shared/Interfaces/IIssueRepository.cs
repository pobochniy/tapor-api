using Tapor.Shared.Dtos;

namespace Tapor.Shared.Interfaces;

public interface IIssueRepository
{
    Task<long> Create(IssueDto dto, CancellationToken ct);
    IAsyncEnumerable<IssueDto> GetList(CancellationToken ct);
    Task<IssueDto?> Details(long id);
}