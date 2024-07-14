using System.ComponentModel.DataAnnotations;

namespace Tapor.Shared.Dtos.Auth;

public class RegisterDto : UserBaseDto
{
    [Required]
    public string Password { get; set; }

    [Compare("Password")]
    public string PasswordConfirm { get; set; }
}