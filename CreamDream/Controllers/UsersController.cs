using CreamDream.Database;
using CreamDream.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly CreamDreamDbContext _context;

    public UsersController(CreamDreamDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all users from the database
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Email,
                u.Phone,
                u.CreatedAt,
                UserType = u is Customer ? "Customer" : u is AuthorizedUser ? "AuthorizedUser" : "User"
            })
            .ToListAsync();

        return Ok(users);
    }
}
