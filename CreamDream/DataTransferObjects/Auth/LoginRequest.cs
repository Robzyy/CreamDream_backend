using System.ComponentModel.DataAnnotations;

namespace CreamDream.DataTransferObjects.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Username or email is required")]
    public string UsernameOrEmail { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}
