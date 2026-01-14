using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly CreamDreamDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthService(CreamDreamDbContext context, IJwtService jwtService, IConfiguration configuration)
    {
        _context = context;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<(string AccessToken, string RefreshToken, int UserId)> RegisterAsync(
        string username, string email, string password, string role = "Customer")
    {
        // validare rol
        if (role != "Customer" && role != "Admin")
            throw new ArgumentException("Invalid role. Must be 'Customer' or 'Admin'");

        if (await UsernameExistsAsync(username))
            throw new InvalidOperationException("Username already exists");

        if (await EmailExistsAsync(email))
            throw new InvalidOperationException("Email already exists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTime.UtcNow.ToString("o"),
            UpdatedAt = DateTime.UtcNow.ToString("o")
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Username, user.Email, user.Role);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7")).ToString("o"),
            CreatedAt = DateTime.UtcNow.ToString("o"),
            IsRevoked = 0
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return (accessToken, refreshToken, user.Id);
    }

    public async Task<(string AccessToken, string RefreshToken, int UserId)?> LoginAsync(string usernameOrEmail, string password)
    {
        var user = await _context.Users
            .Where(u => u.DeletedAt == null && (u.Username == usernameOrEmail || u.Email == usernameOrEmail))
            .FirstOrDefaultAsync();

        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Username, user.Email, user.Role);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7")).ToString("o"),
            CreatedAt = DateTime.UtcNow.ToString("o"),
            IsRevoked = 0
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return (accessToken, refreshToken, user.Id);
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.IsRevoked == 0);

        if (token == null)
            return false;

        token.IsRevoked = 1;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.IsRevoked == 0);

        if (token == null)
            return null;

        if (DateTime.Parse(token.ExpiresAt) < DateTime.UtcNow)
            return null;

        if (token.User.DeletedAt != null)
            return null;

        var newAccessToken = _jwtService.GenerateAccessToken(
            token.User.Id, token.User.Username, token.User.Email, token.User.Role);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        token.IsRevoked = 1;

        var newRefreshTokenEntity = new RefreshToken
        {
            UserId = token.UserId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7")).ToString("o"),
            CreatedAt = DateTime.UtcNow.ToString("o"),
            IsRevoked = 0
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

    public async Task<bool> DeleteAccountAsync(int userId, string password)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId && u.DeletedAt == null)
            .FirstOrDefaultAsync();

        if (user == null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return false;

        // Soft delete
        user.DeletedAt = DateTime.UtcNow.ToString("o");
        user.UpdatedAt = DateTime.UtcNow.ToString("o");

        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.IsRevoked == 0)
            .ToListAsync();

        foreach (var token in refreshTokens)
        {
            token.IsRevoked = 1;
        }

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users
            .Where(u => u.DeletedAt == null)
            .AnyAsync(u => u.Username == username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .Where(u => u.DeletedAt == null)
            .AnyAsync(u => u.Email == email);
    }
}
