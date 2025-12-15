using MegaMarket.BlazorUI.Services.Auth;
using System.Net.Http.Headers;
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
        private readonly AuthService _authService;

        public CustomerService(HttpClient httpClient, IConfiguration config, AuthService authService)
        {
            _httpClient = httpClient;
            _config = config;
            _apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7284";
            _authService = authService;
        }

        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            await AddAuthorizationHeaderAsync();
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
            await AddAuthorizationHeaderAsync();
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
            await AddAuthorizationHeaderAsync();
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
            await AddAuthorizationHeaderAsync();
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
            await AddAuthorizationHeaderAsync();
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

        private async Task AddAuthorizationHeaderAsync()
        {
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
