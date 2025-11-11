using MegaMarket.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMarket.Data.Repositories
{
    public interface ICustomerRepository
    {
        // get all customers
        Task<IEnumerable<Customer>> GetCustomersAsync();

        // create a customer 
        Task<Customer> CreateCustomerAsync(Customer customer);

        //get a customer by id
        Task<Customer?> GetCustomerByIdAsync(int id);

        // check phone number is exsit 
        Task<bool> IsExistPhoneAsync(string phoneNumber);

        // update information of the customer
        Task<Customer?> UpdateCustomerAsync(Customer customer);

        // delete a customer
        Task<bool> DeleteCustomerAsync(int id);

        // filter customer by field
        Task<IEnumerable<Customer>> FilterCustomersAsync(string? fullName, string? phone, string? rank);
    }
}
