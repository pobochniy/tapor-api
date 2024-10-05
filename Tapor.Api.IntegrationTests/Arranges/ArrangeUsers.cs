using Tapor.Shared.Dtos.Auth;

namespace Tapor.Api.IntegrationTests.Arranges;

public static class ArrangeUsers
{
    public static readonly RegisterDto Vlad = new RegisterDto
    {
        UserName = "Vlad",
        Email = "vlad@email.ru",
        Phone = "+79091112234",
        Password = "123",
        PasswordConfirm = "123"
    };
}