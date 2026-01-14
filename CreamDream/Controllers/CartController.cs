using CreamDream.DataTransferObjects.Cart;
using CreamDream.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CreamDream.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<ActionResult<CartResponse>> GetCart()
    {
        var userId = GetUserIdFromClaims();

        try
        {
            var cart = await _cartService.GetOrCreateCartAsync(userId);
            var total = await _cartService.GetCartTotalAsync(userId);
            var itemCount = await _cartService.GetCartItemCountAsync(userId);

            var response = new CartResponse
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(ci => new CartItemResponse
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductPrice = (decimal)ci.Product.Price,
                    ImageUrl = ci.Product.ImageUrl,
                    Quantity = ci.Quantity,
                    Subtotal = (decimal)(ci.Product.Price * ci.Quantity),
                    IsAvailable = ci.Product.IsAvailable == 1
                }).ToList(),
                TotalAmount = (decimal)total,
                ItemCount = itemCount,
                UpdatedAt = cart.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddItemToCart([FromBody] AddToCartRequest request)
    {
        var userId = GetUserIdFromClaims();

        try
        {
            await _cartService.AddItemToCartAsync(userId, request.ProductId, request.Quantity);
            
            var cart = await _cartService.GetOrCreateCartAsync(userId);
            var total = await _cartService.GetCartTotalAsync(userId);
            var itemCount = await _cartService.GetCartItemCountAsync(userId);

            var response = new CartResponse
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(ci => new CartItemResponse
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductPrice = (decimal)ci.Product.Price,
                    ImageUrl = ci.Product.ImageUrl,
                    Quantity = ci.Quantity,
                    Subtotal = (decimal)(ci.Product.Price * ci.Quantity),
                    IsAvailable = ci.Product.IsAvailable == 1
                }).ToList(),
                TotalAmount = (decimal)total,
                ItemCount = itemCount,
                UpdatedAt = cart.UpdatedAt
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("items/{productId}")]
    public async Task<ActionResult<CartResponse>> UpdateCartItem(int productId, [FromBody] UpdateCartItemRequest request)
    {
        var userId = GetUserIdFromClaims();

        try
        {
            await _cartService.UpdateCartItemQuantityAsync(userId, productId, request.Quantity);

            var cart = await _cartService.GetOrCreateCartAsync(userId);
            var total = await _cartService.GetCartTotalAsync(userId);
            var itemCount = await _cartService.GetCartItemCountAsync(userId);

            var response = new CartResponse
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(ci => new CartItemResponse
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductPrice = (decimal)ci.Product.Price,
                    ImageUrl = ci.Product.ImageUrl,
                    Quantity = ci.Quantity,
                    Subtotal = (decimal)(ci.Product.Price * ci.Quantity),
                    IsAvailable = ci.Product.IsAvailable == 1
                }).ToList(),
                TotalAmount = (decimal)total,
                ItemCount = itemCount,
                UpdatedAt = cart.UpdatedAt
            };

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<CartResponse>> RemoveCartItem(int productId)
    {
        var userId = GetUserIdFromClaims();

        try
        {
            await _cartService.RemoveItemFromCartAsync(userId, productId);

            var cart = await _cartService.GetOrCreateCartAsync(userId);
            var total = await _cartService.GetCartTotalAsync(userId);
            var itemCount = await _cartService.GetCartItemCountAsync(userId);

            var response = new CartResponse
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = cart.CartItems.Select(ci => new CartItemResponse
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductPrice = (decimal)ci.Product.Price,
                    ImageUrl = ci.Product.ImageUrl,
                    Quantity = ci.Quantity,
                    Subtotal = (decimal)(ci.Product.Price * ci.Quantity),
                    IsAvailable = ci.Product.IsAvailable == 1
                }).ToList(),
                TotalAmount = (decimal)total,
                ItemCount = itemCount,
                UpdatedAt = cart.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserIdFromClaims();

        try
        {
            await _cartService.ClearCartAsync(userId);
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
