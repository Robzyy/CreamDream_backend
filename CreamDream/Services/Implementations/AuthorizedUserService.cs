using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class AuthorizedUserService : IAuthorizedUserService
{
    private readonly CreamDreamDbContext _context;

    public AuthorizedUserService(CreamDreamDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuthorizedUserDto>> GetAllAuthorizedUsersAsync()
    {
        var users = await _context.AuthorizedUsers.ToListAsync();
        return users.Select(MapToDto);
    }

    public async Task<AuthorizedUserDto?> GetAuthorizedUserByIdAsync(int id)
    {
        var user = await _context.AuthorizedUsers.FindAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<AuthorizedUserDto> CreateAuthorizedUserAsync(CreateAuthorizedUserDto createDto)
    {
        var user = new AuthorizedUser
        {
            Name = createDto.Name,
            Email = createDto.Email,
            Phone = createDto.Phone,
            RolesBitmask = createDto.RolesBitmask
        };

        _context.AuthorizedUsers.Add(user);
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<AuthorizedUserDto?> UpdateAuthorizedUserAsync(int id, UpdateAuthorizedUserDto updateDto)
    {
        var user = await _context.AuthorizedUsers.FindAsync(id);
        if (user == null)
            return null;

        user.Name = updateDto.Name;
        user.Email = updateDto.Email;
        user.Phone = updateDto.Phone;
        user.RolesBitmask = updateDto.RolesBitmask;

        _context.AuthorizedUsers.Update(user);
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<bool> DeleteAuthorizedUserAsync(int id)
    {
        var user = await _context.AuthorizedUsers.FindAsync(id);
        if (user == null)
            return false;

        _context.AuthorizedUsers.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }

    private AuthorizedUserDto MapToDto(AuthorizedUser user)
    {
        return new AuthorizedUserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            RolesBitmask = user.RolesBitmask,
            CreatedAt = user.CreatedAt
        };
    }
}
