using MegaMarket.Data.Models;
using MegaMarket.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
namespace MegaMarket.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly MegaMarketDbContext _context;
        public CustomerRepository(MegaMarketDbContext context)
        {
            _context = context;
        }

        // get all customers
        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            // get all customers from db 
            var customers = await _context.Customers.ToListAsync();
            return customers;
        }

        // create a customer 
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            try
            {
                // add customer in db
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (Exception ex)
            {
                // log error 
                throw new Exception($"Error when adding customer in repository: {ex.Message}");
            }
        }

        // get customer by id 
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            // find customer in db
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                return customer;
            }
            return null;
        }

        // check phone exsit
        public async Task<bool> IsExistPhoneAsync(string phoneNumber)
        {
            return await _context.Customers.AnyAsync(c => c.Phone == phoneNumber);
        }

        // update customer
        public async Task<Customer?> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                // get customer in db
                var customer_exist = await _context.Customers.FindAsync(customer.CustomerId);
                if(customer_exist == null)
                {
                    return null;
                }

                // save changes in customer
                customer_exist.FullName = customer.FullName;
                customer_exist.Phone = customer.Phone;
                customer_exist.Email = customer.Email;
                customer_exist.Rank = customer.Rank;
                customer_exist.Points = customer.Points;
                // save in db
                await _context.SaveChangesAsync();
                return customer_exist;
            }
            catch (Exception ex) {
                throw new Exception($"Error when updating customer in repository: {ex.Message}");
            }
        }

        // delete customer
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            // get customer in db
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return false;
            }

            try
            {
                //delete from db 
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // filter customer by fullname, phone, rank
        public async Task<IEnumerable<Customer>> FilterCustomersAsync(string? fullName, string? phone, string? rank)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                query = query.Where(c => c.FullName != null && c.FullName.ToLower().Contains(fullName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(c => c.Phone != null && c.Phone.ToLower().Contains(phone.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(rank))
            {
                query = query.Where(c => c.Rank == rank);
            }

            return await query.ToListAsync();
        }


        // update point 
        public async Task<bool> UpdateCustomerPointsAsync(int customerId, int newPoints)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return false;

            customer.Points = newPoints;

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
