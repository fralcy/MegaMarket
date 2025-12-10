using System.Net.Http.Json;

namespace MegaMarket.BlazorUI.Services.CustomerLoyalty
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int Points { get; set; }
        public string? Rank { get; set; }
    }

    public class CreateCustomerDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int Points { get; set; } = 0;
    }

    public class UpdateCustomerDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int Points { get; set; }
        public string? Rank { get; set; }
    }

    public class CustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private string _apiBaseUrl;

        public CustomerService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7284";
        }

        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/customers");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<CustomerDto>>() ?? new();
                }
                return new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching customers: {ex.Message}", ex);
            }
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/customers/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomerDto>();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching customer {id}: {ex.Message}", ex);
            }
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/customers", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomerDto>() ?? new();
                }
                throw new Exception("Failed to create customer");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating customer: {ex.Message}", ex);
            }
        }

        public async Task<CustomerDto> UpdateCustomerAsync(int id, UpdateCustomerDto request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/customers/{id}", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomerDto>() ?? new();
                }
                throw new Exception("Failed to update customer");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating customer: {ex.Message}", ex);
            }
        }

        public async Task DeleteCustomerAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/customers/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to delete customer");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting customer: {ex.Message}", ex);
            }
        }
    }
}
