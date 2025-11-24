using MegaMarket.API.DTOs.Customers;
using System.Diagnostics.Eventing.Reader;

namespace MegaMarket.API.Services
{
    public interface ICustomerService
    {
        // get all customers
        Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync();

        // create a customer
        Task<CustomerResponseDto?> CreateCustomerAsync(CreateCustomerRequestDto customer_dto);

        // get a customer
        Task<CustomerResponseDto?> GetCustomerById(int customer_id);

        // update cutomer
        Task<CustomerResponseDto?> UpdateCustomerAsync(int customerId, UpdateCustomerRequestDto customer_dto);

        // delete customer
        Task<bool> DeleteCustomerAsync(int customer_id);

        // filter customer
        Task<IEnumerable<CustomerResponseDto>> FilterCustomersAsync(FilterCustomerRequestDto filter);

    }
}
