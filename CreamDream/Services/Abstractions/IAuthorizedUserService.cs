using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;

namespace CreamDream.Services.Abstractions;

public interface IAuthorizedUserService
{
    Task<IEnumerable<AuthorizedUserDto>> GetAllAuthorizedUsersAsync();
    Task<AuthorizedUserDto?> GetAuthorizedUserByIdAsync(int id);
    Task<AuthorizedUserDto> CreateAuthorizedUserAsync(CreateAuthorizedUserDto createDto);
    Task<AuthorizedUserDto?> UpdateAuthorizedUserAsync(int id, UpdateAuthorizedUserDto updateDto);
    Task<bool> DeleteAuthorizedUserAsync(int id);
}
