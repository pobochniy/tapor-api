using Tapor.Shared.Dtos;

namespace Tapor.Shared;

public interface IIssueRepository
{
    long Create(IssueDto dto);
}