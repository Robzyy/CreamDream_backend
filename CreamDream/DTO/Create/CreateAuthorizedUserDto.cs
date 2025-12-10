namespace CreamDream.DTO.Create;

public class CreateAuthorizedUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int RolesBitmask { get; set; }
}
