using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.DTO.Auth;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Abstractions;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
