using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;
using CreamDream.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CreamDream.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizedUsersController : ControllerBase
{
    private readonly IAuthorizedUserService _authorizedUserService;

    public AuthorizedUsersController(IAuthorizedUserService authorizedUserService)
    {
        _authorizedUserService = authorizedUserService;
    }

    /// <summary>
    /// Get all authorized users (staff, admins, etc.)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuthorizedUserDto>>> GetAll()
    {
        var users = await _authorizedUserService.GetAllAuthorizedUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get an authorized user by id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorizedUserDto>> GetById(int id)
    {
        var user = await _authorizedUserService.GetAuthorizedUserByIdAsync(id);
        if (user == null)
            return NotFound($"Authorized user with id {id} not found");

        return Ok(user);
    }

    /// <summary>
    /// Create a new authorized user (staff, admin, etc.)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AuthorizedUserDto>> Create([FromBody] CreateAuthorizedUserDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _authorizedUserService.CreateAuthorizedUserAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Update an authorized user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<AuthorizedUserDto>> Update(int id, [FromBody] UpdateAuthorizedUserDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _authorizedUserService.UpdateAuthorizedUserAsync(id, updateDto);
        if (user == null)
            return NotFound($"Authorized user with id {id} not found");

        return Ok(user);
    }

    /// <summary>
    /// Delete an authorized user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _authorizedUserService.DeleteAuthorizedUserAsync(id);
        if (!success)
            return NotFound($"Authorized user with id {id} not found");

        return NoContent();
    }
}
