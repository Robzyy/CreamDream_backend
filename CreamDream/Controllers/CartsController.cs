using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;
using CreamDream.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreamDream.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartsController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Get cart by customer ID
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<CartDto>> GetCart(int customerId)
    {
        var cart = await _cartService.GetCartByCustomerIdAsync(customerId);
        if (cart == null)
            return NotFound("Cart not found.");

        return Ok(cart);
    }

    /// <summary>
    /// Add item to cart with stock validation
    /// </summary>
    [HttpPost("customer/{customerId}/items")]
    public async Task<ActionResult<CartItemDto>> AddItemToCart(int customerId, [FromBody] CreateCartItemDto dto)
    {
        try
        {
            var cartItem = await _cartService.AddItemToCartAsync(customerId, dto);
            return Created($"api/carts/customer/{customerId}/items/{cartItem.Id}", cartItem);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update cart item quantity
    /// </summary>
    [HttpPut("customer/{customerId}/items/{cartItemId}")]
    public async Task<ActionResult<CartItemDto>> UpdateCartItem(int customerId, int cartItemId,
        [FromBody] UpdateCartItemDto dto)
    {
        try
        {
            var cartItem = await _cartService.UpdateCartItemAsync(customerId, cartItemId, dto);
            return Ok(cartItem);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpDelete("customer/{customerId}/items/{cartItemId}")]
    public async Task<IActionResult> RemoveItemFromCart(int customerId, int cartItemId)
    {
        try
        {
            await _cartService.RemoveItemFromCartAsync(customerId, cartItemId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Clear all items from cart
    /// </summary>
    [HttpDelete("customer/{customerId}")]
    public async Task<IActionResult> ClearCart(int customerId)
    {
        try
        {
            await _cartService.ClearCartAsync(customerId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Validate cart stock before checkout
    /// </summary>
    [HttpPost("customer/{customerId}/validate")]
    public async Task<ActionResult<CartDto>> ValidateCart(int customerId)
    {
        try
        {
            var cart = await _cartService.ValidateCartStockAsync(customerId);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
