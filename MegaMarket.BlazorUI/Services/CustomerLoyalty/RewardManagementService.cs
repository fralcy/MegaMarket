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

        public RewardManagementService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7224";
        }

        public async Task<List<RewardManagementDto>> GetAllRewardsAsync()
        {
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
    }
}
