using MegaMarket.API.DTOs.Customers;
using MegaMarket.Data.Models;
using MegaMarket.Data.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata.Ecma335;
namespace MegaMarket.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // get all customers
        public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync()
        {
            // get all customers from db
            var customers = await _customerRepository.GetCustomersAsync();
            // convert sang dto
            var response = customers.Select(c => new CustomerResponseDto
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                Phone = c.Phone,
                Points = c.Points,
                Email = c.Email,
                Rank = c.Rank,
            });

            return response;

        }

        // create a customer
        public async Task<CustomerResponseDto?> CreateCustomerAsync(CreateCustomerRequestDto customer_dto)
        {
            try
            {
                // check exist phone
                if (!string.IsNullOrWhiteSpace(customer_dto.Phone))
                {
                    var isPhoneExist = await _customerRepository.IsExistPhoneAsync(customer_dto.Phone);
                    if (isPhoneExist)
                    {
                        throw new Exception("Phone already exists.");
                    }
                }

                // create a customer
                var customer = new Customer
                {
                    FullName = customer_dto.FullName,
                    Phone = customer_dto.Phone,
                    Email = customer_dto.Email,
                    Points = customer_dto.Points
                };

                // save in db by repository
                var createdCustomer = await _customerRepository.CreateCustomerAsync(customer);

                // convert to response dto 
                var response = new CustomerResponseDto
                {
                    CustomerId = createdCustomer.CustomerId,
                    FullName = createdCustomer.FullName,
                    Phone = createdCustomer.Phone,
                    Email = createdCustomer.Email,
                    Points = createdCustomer.Points,
                    Rank = createdCustomer.Rank
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when adding customer in service: {ex.Message}");
            }
        }

        // get customer by id
        public async Task<CustomerResponseDto?> GetCustomerById(int customer_id)
        {
            // get customer by id
            var customer = await _customerRepository.GetCustomerByIdAsync(customer_id);
            // check exist
            if(customer == null)
            {
                return null;
            }
            // return for client
            return new CustomerResponseDto
            {
                CustomerId = customer.CustomerId,
                FullName = customer.FullName,
                Phone = customer.Phone,
                Email = customer.Email,
                Points = customer.Points,
                Rank = customer.Rank
            };
        }

        // update customer 
        public async Task<CustomerResponseDto?> UpdateCustomerAsync(int customerId, UpdateCustomerRequestDto customer_dto)
        {
            try
            {
                // get customer exi
                var customer_exist = await _customerRepository.GetCustomerByIdAsync(customerId);
                if(customer_exist == null)
                {
                    return null;
                }

                if (customer_exist.Phone != null)
                {
                        // check exist phone 
                    if (!string.IsNullOrWhiteSpace(customer_dto.Phone) && customer_exist.Phone != customer_dto.Phone )
                    {
                        var isPhoneExist = await _customerRepository.IsExistPhoneAsync(customer_dto.Phone);
                        if (isPhoneExist)
                        {
                            return null;
                        }
                    }
                } else
                {
                    if (!string.IsNullOrWhiteSpace(customer_dto.Phone))
                    {
                        var isPhoneExist = await _customerRepository.IsExistPhoneAsync(customer_dto.Phone);
                        if (isPhoneExist)
                        {
                            return null;
                        }
                    }
                }



                //create customer
                var customer = new Customer
                {
                    CustomerId = customerId, // assign customerId
                    FullName = customer_dto.FullName,
                    Phone = customer_dto.Phone,
                    Email = customer_dto.Email,
                    Points = customer_dto.Points,
                    Rank = customer_dto.Rank!
                };

                // save in db by repo
                var updatedCustomer = await _customerRepository.UpdateCustomerAsync(customer);
                if (updatedCustomer == null)
                {
                    return null;
                }

                // convert to dto 
                var response = new CustomerResponseDto
                {
                    CustomerId = updatedCustomer.CustomerId,
                    FullName = updatedCustomer.FullName,
                    Phone = updatedCustomer.Phone,
                    Email = updatedCustomer.Email,
                    Points = updatedCustomer.Points,
                    Rank = updatedCustomer.Rank
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when updating customer in service: {ex.Message}");
            }
        }

        // delete customer by id
        public async Task<bool> DeleteCustomerAsync(int customer_id)
        {
            try
            {
                var result = await _customerRepository.DeleteCustomerAsync(customer_id);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // filter customer 
        public async Task<IEnumerable<CustomerResponseDto>> FilterCustomersAsync(FilterCustomerRequestDto filter)
        {
            var customers = await _customerRepository.FilterCustomersAsync(
                filter.FullName,
                filter.Phone,
                filter.Rank
            );

            return customers.Select(c => new CustomerResponseDto
            {
                CustomerId = c.CustomerId,
                FullName = c.FullName,
                Phone = c.Phone,
                Email = c.Email,
                Points = c.Points,
                Rank = c.Rank
            });
        }

    }
}
