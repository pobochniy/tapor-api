using Microsoft.Extensions.Logging;
using Tapor.DB;
using Tapor.Shared;
using Tapor.Shared.Dtos;

namespace Tapor.Services.Tests;

public class Tests
{
    [Test]
    public void WhenReporterIsEmpty_ShouldTakeCurrentUser()
    {
        // Arrange
        var repository = new IssueTestRepository();
        var service = new IssueService(repository);
        var model = new IssueDto();
        var currentUser = Guid.NewGuid();
        
        // Act
        var issueId = service.Create(model, currentUser);
        
        // Assert
        var issue = repository.GetIssue(issueId);
        Assert.That(issue.Reporter, Is.EqualTo(currentUser));
    }

    private class IssueTestRepository: IIssueRepository
    {
        private List<IssueDto> _repo = new List<IssueDto>();
        
        public long Create(IssueDto dto)
        {
            dto.Id = Random.Shared.Next();
            _repo.Add(dto);
            return (long)dto.Id;
        }

        public IssueDto GetIssue(long id)
        {
            return _repo.Single(x => x.Id == id);
        }
    }
}