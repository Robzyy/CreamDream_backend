using CreamDream.DataTransferObjects.Addresses;
using CreamDream.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CreamDream.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet("my-address")]
    public async Task<ActionResult<AddressResponse>> GetMyAddress()
    {
        var userId = GetUserIdFromClaims();

        try
        {
            var address = await _addressService.GetUserAddressAsync(userId);
            if (address == null)
                return NotFound(new { message = "Address not found" });

            var response = new AddressResponse
            {
                Id = address.Id,
                UserId = address.UserId,
                FullName = address.FullName,
                PhoneNumber = address.PhoneNumber,
                StreetAddress = address.StreetAddress,
                City = address.City,
                PostalCode = address.PostalCode,
                Country = address.Country,
                AddressNotes = address.AddressNotes
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("my-address")]
    [HttpPut("my-address")]
    public async Task<ActionResult<AddressResponse>> CreateOrUpdateAddress([FromBody] CreateOrUpdateAddressRequest request)
    {
        var userId = GetUserIdFromClaims();

        try
        {
            var address = await _addressService.CreateOrUpdateAddressAsync(
                userId,
                request.FullName,
                request.PhoneNumber,
                request.StreetAddress,
                request.City,
                request.PostalCode,
                request.Country,
                request.AddressNotes
            );

            var response = new AddressResponse
            {
                Id = address.Id,
                UserId = address.UserId,
                FullName = address.FullName,
                PhoneNumber = address.PhoneNumber,
                StreetAddress = address.StreetAddress,
                City = address.City,
                PostalCode = address.PostalCode,
                Country = address.Country,
                AddressNotes = address.AddressNotes
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("my-address")]
    public async Task<IActionResult> DeleteMyAddress()
    {
        var userId = GetUserIdFromClaims();

        try
        {
            var success = await _addressService.DeleteAddressAsync(userId);
            if (!success)
                return NotFound(new { message = "Address not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
