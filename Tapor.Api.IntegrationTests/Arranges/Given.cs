using Tapor.Api.IntegrationTests.ArrangeEntityBuilders;
using Tapor.DB.Entity;
using Tapor.Shared.Enums;

namespace Tapor.Api.IntegrationTests.Arranges;

public static class Given
{
    public static async Task<HttpClient> ApiClient(Action<ApplicationContext>? dbArrange = null)
    {
        var client = await new ApiApplicationFactory<Program>()
            .SetupApplication(dbArrange, Guid.NewGuid().ToString());
        return client;
    }
    
    public static Profile Admin()
    {
        return new UserBuilder(UserBuilder.SuperAdminName, UserBuilder.SuperAdminEmail, UserBuilder.SuperAdminPhone)
            .WithAllRoles()
            .Please();
    }
    
    public static Profile Vlad(ICollection<RoleEnum> roles)
    {
        return new UserBuilder(UserBuilder.VladName, UserBuilder.VladEmail, UserBuilder.VladPhone)
            .WithRoles(roles)
            .Please();
    }

    public static Issue Issue(Guid userId, int? issueId = null)
    {
        return new IssueBuilder()
            .WithId(issueId)
            .WithAssignee(userId)
            .WithReporter(userId)
            .Please();
    } 
}