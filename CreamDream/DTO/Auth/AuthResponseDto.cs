namespace CreamDream.DTO.Auth;

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public string? Token { get; set; }
}
