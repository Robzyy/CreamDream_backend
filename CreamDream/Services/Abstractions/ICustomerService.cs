using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;

namespace CreamDream.Services.Abstractions;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createDto);
    Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateDto);
    Task<bool> DeleteCustomerAsync(int id);
}
