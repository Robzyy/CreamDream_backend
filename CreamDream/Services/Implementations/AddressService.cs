using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class AddressService : IAddressService
{
    private readonly CreamDreamDbContext _context;

    public AddressService(CreamDreamDbContext context)
    {
        _context = context;
    }

    public async Task<Address?> GetUserAddressAsync(int userId)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<Address> CreateOrUpdateAddressAsync(
        int userId, string fullName, string phoneNumber, string streetAddress, 
        string city, string postalCode, string country, string? addressNotes = null)
    {
        // daca exista deja
        var existingAddress = await _context.Addresses
            .FirstOrDefaultAsync(a => a.UserId == userId);

        // daca da actualizeaza 
        if (existingAddress != null)
        {
            existingAddress.FullName = fullName;
            existingAddress.PhoneNumber = phoneNumber;
            existingAddress.StreetAddress = streetAddress;
            existingAddress.City = city;
            existingAddress.PostalCode = postalCode;
            existingAddress.Country = country;
            existingAddress.AddressNotes = addressNotes;
            existingAddress.UpdatedAt = DateTime.UtcNow.ToString("o");

            await _context.SaveChangesAsync();
            return existingAddress;
        }
        else
        {
            //daca nu, creeaza o adresa noua
            var address = new Address
            {
                UserId = userId,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                StreetAddress = streetAddress,
                City = city,
                PostalCode = postalCode,
                Country = country,
                AddressNotes = addressNotes,
                CreatedAt = DateTime.UtcNow.ToString("o"),
                UpdatedAt = DateTime.UtcNow.ToString("o")
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }
    }

    public async Task<bool> DeleteAddressAsync(int userId)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (address == null)
            return false;

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
        return true;
    }
}
