namespace CreamDream.Database.Models;

public class AuthorizedUser : User
{
    public int RolesBitmask { get; set; } // Bitmask for roles (e.g., Admin=1, Staff=2, Manager=4)
    // After reading Database Design for mere mortals I realize this was a bad idea but eh
}
