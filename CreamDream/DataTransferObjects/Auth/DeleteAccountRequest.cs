using System.ComponentModel.DataAnnotations;

namespace CreamDream.DataTransferObjects.Auth;

public class DeleteAccountRequest
{
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}
