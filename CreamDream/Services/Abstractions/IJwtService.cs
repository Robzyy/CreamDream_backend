using System.Security.Claims;

namespace CreamDream.Services.Abstractions;

public interface IJwtService
{
    string GenerateAccessToken(int userId, string username, string email, string role);

    string GenerateRefreshToken();

    ClaimsPrincipal? ValidateToken(string token);

    int? GetUserIdFromToken(string token);
}
