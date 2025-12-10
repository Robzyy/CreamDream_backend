namespace CreamDream.DTO.Update;

public class UpdateAuthorizedUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int RolesBitmask { get; set; }
}
