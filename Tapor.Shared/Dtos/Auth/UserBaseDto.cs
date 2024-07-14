using System.ComponentModel.DataAnnotations;

namespace Tapor.Shared.Dtos.Auth;

public class UserBaseDto
{
    public string UserName { get; set; }

    public string Phone { get; set; }

    [Required]
    public string Email { get; set; }
}