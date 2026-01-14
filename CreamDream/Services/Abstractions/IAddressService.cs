using CreamDream.Database.Models;

namespace CreamDream.Services.Abstractions;

public interface IAddressService
{
    Task<Address?> GetUserAddressAsync(int userId);

    /// <summary>
    /// Creates or updates the user's address
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="fullName">Full name for delivery</param>
    /// <param name="phoneNumber">Phone number</param>
    /// <param name="streetAddress">Street address</param>
    /// <param name="city">City</param>
    /// <param name="postalCode">Postal code</param>
    /// <param name="country">Country</param>
    /// <param name="addressNotes">Optional delivery notes</param>
    /// <returns>Created or updated address</returns>
    Task<Address> CreateOrUpdateAddressAsync(int userId, string fullName, string phoneNumber, 
        string streetAddress, string city, string postalCode, string country, string? addressNotes = null);

    Task<bool> DeleteAddressAsync(int userId);
}
