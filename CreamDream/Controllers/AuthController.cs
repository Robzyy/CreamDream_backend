using CreamDream.DataTransferObjects.Auth;
using CreamDream.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CreamDream.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<TokenResponse>> Register([FromBody] RegisterRequest request)
    {
        // Check if username exists
        if (await _authService.UsernameExistsAsync(request.Username))
            return Conflict(new { message = "Username already exists" });

        // Check if email exists
        if (await _authService.EmailExistsAsync(request.Email))
            return Conflict(new { message = "Email already exists" });

        try
        {
            var (accessToken, refreshToken, userId) = await _authService.RegisterAsync(
                request.Username, 
                request.Email, 
                request.Password, 
                request.Role
            );

            var expirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "30");

            return Ok(new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserId = userId,
                Username = request.Username,
                Email = request.Email,
                Role = request.Role,
                ExpiresIn = expirationMinutes * 60
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(
                request.UsernameOrEmail, 
                request.Password
            );

            if (result == null)
                return Unauthorized(new { message = "Invalid credentials" });

            var expirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "30");

            var token = result.Value;
            
            return Ok(new TokenResponse
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                UserId = token.UserId,
                Username = request.UsernameOrEmail, 
                Email = request.UsernameOrEmail,  
                Role = "Customer",      
                ExpiresIn = expirationMinutes * 60
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(
                request.RefreshToken
            );

            if (result == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            var expirationMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "30");

            var token = result.Value;

            return Ok(new TokenResponse
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                UserId = 0,   
                Username = "",
                Email = "",   
                Role = "",     
                ExpiresIn = expirationMinutes * 60
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        try
        {
            await _authService.LogoutAsync(request.RefreshToken);
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("delete-account")]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
    {
        var userId = GetUserIdFromClaims();

        try
        {
            var success = await _authService.DeleteAccountAsync(userId, request.Password);
            if (!success)
                return NotFound(new { message = "User not found or invalid password" });

            return Ok(new { message = "Account deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
