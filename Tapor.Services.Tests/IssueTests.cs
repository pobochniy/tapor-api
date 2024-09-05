using System.Runtime.CompilerServices;
using Tapor.DB;
using Tapor.Shared;
using Tapor.Shared.Dtos;
using Tapor.Shared.Interfaces;

namespace Tapor.Services.Tests;

public class Tests
{
    [Test]
    public async Task WhenReporterIsEmpty_ShouldTakeCurrentUser()
    {
        // Arrange
        var repository = new IssueTestRepository();
        var notificationService = new NotificationService(new NotificationsRepository());
        var service = new IssueService(repository, notificationService);
        var model = new IssueDto();
        var currentUser = Guid.NewGuid();
        
        // Act
        var issueId = await service.Create(model, currentUser, default);
        
        // Assert
        var issue = repository.GetIssue(issueId);
        Assert.That(issue.Reporter, Is.EqualTo(currentUser));
    }

    private class IssueTestRepository: IIssueRepository
    {
        private readonly List<IssueDto> _repo = new();
        
        public Task<long> Create(IssueDto dto, CancellationToken ct = default)
        {
            dto.Id = Random.Shared.Next();
            _repo.Add(dto);
            return Task.FromResult((long)dto.Id);
        }

        public async IAsyncEnumerable<IssueDto> GetList([EnumeratorCancellation] CancellationToken ct)
        {
            foreach (var dto in _repo)
            {
                yield return dto;
            }
        }

        public IssueDto GetIssue(long id)
        {
            return _repo.Single(x => x.Id == id);
        }
    }
}