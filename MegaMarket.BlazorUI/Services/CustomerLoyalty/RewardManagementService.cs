using MegaMarket.BlazorUI.Services.Auth;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace MegaMarket.BlazorUI.Services.CustomerLoyalty
{
    public class RewardManagementDto
    {
        public int RewardId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int PointCost { get; set; }
        public string RewardType { get; set; } = "Gift";
        public decimal? Value { get; set; }
        public int QuantityAvailable { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateRewardDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int PointCost { get; set; }
        public string RewardType { get; set; } = "Gift";
        public decimal? Value { get; set; }
        public int QuantityAvailable { get; set; }
    }

    public class UpdateRewardDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int PointCost { get; set; }
        public string RewardType { get; set; } = "Gift";
        public decimal? Value { get; set; }
        public int QuantityAvailable { get; set; }
    }

    public class RewardManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private string _apiBaseUrl;
        private readonly AuthService _authService;

        public RewardManagementService(HttpClient httpClient, IConfiguration config, AuthService authService)
        {
            _httpClient = httpClient;
            _config = config;
            _apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7224";
            _authService = authService;
        }

        public async Task<List<RewardManagementDto>> GetAllRewardsAsync()
        {
            await AddAuthorizationHeaderAsync();
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/rewards");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<RewardManagementDto>>() ?? new();
                }
                return new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching rewards: {ex.Message}", ex);
            }
        }

        public async Task<RewardManagementDto?> GetRewardByIdAsync(int id)
        {
            await AddAuthorizationHeaderAsync();
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/rewards/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RewardManagementDto>();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching reward {id}: {ex.Message}", ex);
            }
        }

        public async Task<RewardManagementDto> CreateRewardAsync(CreateRewardDto request)
        {
            await AddAuthorizationHeaderAsync();
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/rewards", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RewardManagementDto>() ?? new();
                }
                throw new Exception("Failed to create reward");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating reward: {ex.Message}", ex);
            }
        }

        public async Task<RewardManagementDto> UpdateRewardAsync(int id, UpdateRewardDto request)
        {
            await AddAuthorizationHeaderAsync();
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/rewards/{id}", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<RewardManagementDto>() ?? new();
                }
                throw new Exception("Failed to update reward");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating reward: {ex.Message}", ex);
            }
        }

        public async Task DeleteRewardAsync(int id)
        {
            await AddAuthorizationHeaderAsync();
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/rewards/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to delete reward");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting reward: {ex.Message}", ex);
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
