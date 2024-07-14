using Tapor.Shared.Enums;

namespace Tapor.Shared.Dtos.Auth;

public class UserDto : UserBaseDto
{
    public Guid Id { get; set; }

    public IEnumerable<RoleEnum> Roles { get; set; }
}