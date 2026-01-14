namespace CreamDream.Services.Abstractions;

public interface IAuthService
{
    Task<(string AccessToken, string RefreshToken, int UserId)> RegisterAsync(string username, string email, string password, string role = "Customer");

    Task<(string AccessToken, string RefreshToken, int UserId)?> LoginAsync(string usernameOrEmail, string password);

    Task<bool> LogoutAsync(string refreshToken);

    Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken);

    Task<bool> DeleteAccountAsync(int userId, string password);

    Task<bool> UsernameExistsAsync(string username);

    Task<bool> EmailExistsAsync(string email);
}
