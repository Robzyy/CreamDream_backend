namespace CreamDream.DTO.Entity;

public class AuthorizedUserDto : UserDto
{
    public int RolesBitmask { get; set; }
}
