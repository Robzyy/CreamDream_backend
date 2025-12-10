using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.DTO.Auth;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CreamDream.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly CreamDreamDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(CreamDreamDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email and password are required."
            };

        if (dto.Password != dto.ConfirmPassword)
            return new AuthResponseDto
            {
                Success = false,
                Message = "Passwords do not match."
            };

        if (dto.Password.Length < 6)
            return new AuthResponseDto
            {
                Success = false,
                Message = "Password must be at least 6 characters long."
            };

        // Check if customer already exists
        var existingCustomer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == dto.Email);

        if (existingCustomer != null)
            return new AuthResponseDto
            {
                Success = false,
                Message = "A customer with this email already exists."
            };

        // Create new customer
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Success = true,
            Message = "Registration successful.",
            CustomerId = customer.Id
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return new AuthResponseDto
            {
                Success = false,
                Message = "Email and password are required."
            };

        // Find customer by email
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == dto.Email);

        if (customer == null)
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password."
            };

        // Verify password
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash);
        if (!isPasswordValid)
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password."
            };

        // Generate JWT token
        var token = GenerateJwtToken(customer);

        return new AuthResponseDto
        {
            Success = true,
            Message = "Login successful.",
            CustomerId = customer.Id,
            Token = token
        };
    }

    private string GenerateJwtToken(Customer customer)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "CreamDream";
        var audience = jwtSettings["Audience"] ?? "CreamDreamCustomers";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "1440");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Email, customer.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, customer.Name ?? string.Empty),
            new Claim("CustomerId", customer.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
