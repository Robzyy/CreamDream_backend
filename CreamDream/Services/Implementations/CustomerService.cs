using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class CustomerService : ICustomerService
{
    private readonly CreamDreamDbContext _context;

    public CustomerService(CreamDreamDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _context.Customers.ToListAsync();
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        return customer != null ? MapToDto(customer) : null;
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createDto)
    {
        var customer = new Customer
        {
            Name = createDto.Name,
            Email = createDto.Email,
            Phone = createDto.Phone,
            Address = createDto.Address
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return MapToDto(customer);
    }

    public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateDto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return null;

        customer.Name = updateDto.Name;
        customer.Email = updateDto.Email;
        customer.Phone = updateDto.Phone;
        customer.Address = updateDto.Address;

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();

        return MapToDto(customer);
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
            return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return true;
    }

    private CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            CreatedAt = customer.CreatedAt
        };
    }
}
