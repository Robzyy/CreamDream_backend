using CreamDream.DataTransferObjects.Products;
using CreamDream.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreamDream.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetProducts(
        [FromQuery] int? typeId = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] bool? available = null,
        [FromQuery] string? search = null)
    {
        try
        {
            var products = await _productService.GetProductsAsync(typeId, categoryId, available);

            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = await _productService.SearchProductsAsync(search);
            }

            var response = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = (decimal)p.Price,
                ImageUrl = p.ImageUrl,
                IsAvailable = p.IsAvailable == 1,
                ProductType = new ProductTypeResponse
                {
                    Id = p.ProductType.Id,
                    Name = p.ProductType.Name,
                    Description = p.ProductType.Description
                },
                Category = p.Category != null ? new CategoryResponse
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    Description = p.Category.Description
                } : null,
                CreatedAt = p.CreatedAt.ToString(),
                UpdatedAt = p.UpdatedAt.ToString()
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = (decimal)product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable == 1,
                ProductType = new ProductTypeResponse
                {
                    Id = product.ProductType.Id,
                    Name = product.ProductType.Name,
                    Description = product.ProductType.Description
                },
                Category = product.Category != null ? new CategoryResponse
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description
                } : null,
                CreatedAt = product.CreatedAt.ToString(),
                UpdatedAt = product.UpdatedAt.ToString()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("types")]
    public async Task<ActionResult<List<ProductTypeResponse>>> GetProductTypes()
    {
        try
        {
            var types = await _productService.GetProductTypesAsync();
            var response = types.Select(t => new ProductTypeResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("categories")]
    public async Task<ActionResult<List<CategoryResponse>>> GetCategories()
    {
        try
        {
            var categories = await _productService.GetCategoriesAsync();
            var response = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            var product = await _productService.CreateProductAsync(
                request.Name,
                request.Description,
                (double)request.Price,
                request.ProductTypeId,
                request.CategoryId,
                request.ImageUrl
            );

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = (decimal)product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable == 1,
                ProductType = new ProductTypeResponse
                {
                    Id = product.ProductType.Id,
                    Name = product.ProductType.Name,
                    Description = product.ProductType.Description
                },
                Category = product.Category != null ? new CategoryResponse
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description
                } : null,
                CreatedAt = product.CreatedAt.ToString(),
                UpdatedAt = product.UpdatedAt.ToString()
            };

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, response);
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

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            // Get existing product first if price is not provided
            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
                return NotFound(new { message = "Product not found" });

            var product = await _productService.UpdateProductAsync(
                id,
                request.Name ?? existingProduct.Name,
                request.Description ?? existingProduct.Description,
                request.Price.HasValue ? (double)request.Price.Value : existingProduct.Price,
                request.ProductTypeId ?? existingProduct.ProductTypeId,
                request.CategoryId ?? existingProduct.CategoryId,
                request.ImageUrl ?? existingProduct.ImageUrl
            );

            if (product == null)
                return NotFound(new { message = "Product not found" });

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = (decimal)product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable == 1,
                ProductType = new ProductTypeResponse
                {
                    Id = product.ProductType.Id,
                    Name = product.ProductType.Name,
                    Description = product.ProductType.Description
                },
                Category = product.Category != null ? new CategoryResponse
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description
                } : null,
                CreatedAt = product.CreatedAt.ToString(),
                UpdatedAt = product.UpdatedAt.ToString()
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
                return NotFound(new { message = "Product not found" });

            return NoContent();
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

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/toggle-availability")]
    public async Task<ActionResult<ProductResponse>> ToggleAvailability(int id)
    {
        try
        {
            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
                return NotFound(new { message = "Product not found" });

            // Toggle availability
            var newAvailability = existingProduct.IsAvailable == 1 ? false : true;
            await _productService.ToggleProductAvailabilityAsync(id, newAvailability);

            // Fetch updated product
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            var response = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = (decimal)product.Price,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable == 1,
                ProductType = new ProductTypeResponse
                {
                    Id = product.ProductType.Id,
                    Name = product.ProductType.Name,
                    Description = product.ProductType.Description
                },
                Category = product.Category != null ? new CategoryResponse
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description
                } : null,
                CreatedAt = product.CreatedAt.ToString(),
                UpdatedAt = product.UpdatedAt.ToString()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
