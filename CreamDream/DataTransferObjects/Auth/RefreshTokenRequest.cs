using System.ComponentModel.DataAnnotations;

namespace CreamDream.DataTransferObjects.Auth;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = null!;
}
