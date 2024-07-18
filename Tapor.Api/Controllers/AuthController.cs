using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Tapor.Services;
using Tapor.Shared.Dtos.Auth;

namespace Tapor.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController: ControllerBase
{
    private readonly AuthService _service;

    public AuthController()
    {
        _service = new AuthService();
    }

    [HttpPost]
    [Produces(typeof(UserDto))]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        try
        {
            _service.Register(model);
            var user = _service.LogIn(new LoginDto {Login = model.UserName, Password = model.Password});
            await SignInAsync(user);

            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest($"{e.GetType()}: {e.Message}");
        }
    }

    [HttpPost]
    [Produces(typeof(UserDto))]
    public async Task<IActionResult> LogIn([FromBody]LoginDto model)
    {
        try
        {
            var user = _service.LogIn(model);
            await SignInAsync(user);

            return Ok(user);
        }
        catch (UnauthorizedAccessException)
        {
            ModelState.AddModelError("", "Неправильное имя пользователя или пароль");
            return UnprocessableEntity(ModelState);
        }
        catch (Exception e)
        {
            return BadRequest($"{e.GetType()}: {e.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return new NoContentResult();
    }

    private async Task SignInAsync(UserDto user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, ((int) role).ToString())));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}