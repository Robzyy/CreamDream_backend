namespace CreamDream.Database.Models;

public class AuthorizedUser : User
{
    public int RolesBitmask { get; set; } // Bitmask for roles (e.g., Admin=1, Staff=2, Manager=4)
}
