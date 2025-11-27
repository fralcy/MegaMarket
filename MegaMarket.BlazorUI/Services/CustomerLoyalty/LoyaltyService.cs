using System.Net.Http.Json;

namespace MegaMarket.BlazorUI.Services.CustomerLoyalty
{
    public class CustomerRewardDto
    {
        public int RedemptionId { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int RewardId { get; set; }
        public string? RewardName { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime RedeemedAt { get; set; }
        public string? Status { get; set; }
        public DateTime? UsedAt { get; set; }
    }

    public class RedeemRewardDto
    {
        public int CustomerId { get; set; }
        public int RewardId { get; set; }
        public int InvoiceId { get; set; } = 0;
    }

    public class PointHistoryDto
    {
        public int TransactionId { get; set; }
        public int PointChange { get; set; }
        public string? TransactionType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public int? InvoiceId { get; set; }
        public string? CustomerName { get; set; }
    }

    public class LoyaltyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private string _apiBaseUrl;

        public LoyaltyService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7224";
        }

        public async Task<List<CustomerRewardDto>> GetAllRewardsAsync(string? status = null, int? customerId = null)
        {
            try
            {
                var url = $"{_apiBaseUrl}/api/customerrewards";
                if (!string.IsNullOrWhiteSpace(status) || customerId.HasValue)
                {
                    var query = new List<string>();
                    if (!string.IsNullOrWhiteSpace(status))
                        query.Add($"status={status}");
                    if (customerId.HasValue)
                        query.Add($"customerId={customerId.Value}");
                    url += "?" + string.Join("&", query);
                }

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<CustomerRewardDto>>() ?? new();
                }
                return new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching rewards: {ex.Message}", ex);
            }
        }

        public async Task<List<CustomerRewardDto>> GetCustomerRewardsAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/customerrewards/customer/{customerId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<CustomerRewardDto>>() ?? new();
                }
                return new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching customer rewards: {ex.Message}", ex);
            }
        }

        public async Task<CustomerRewardDto> RedeemRewardAsync(RedeemRewardDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/customerrewards/redeem", request);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    return await response.Content.ReadFromJsonAsync<CustomerRewardDto>() ?? new();
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to redeem reward: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error redeeming reward: {ex.Message}", ex);
            }
        }

        public async Task<CustomerRewardDto> UseRewardAsync(int rewardId)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/customerrewards/{rewardId}/use", new { });
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CustomerRewardDto>() ?? new();
                }
                throw new Exception("Failed to use reward");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error using reward: {ex.Message}", ex);
            }
        }

        public async Task DeleteRewardAsync(int rewardId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/customerrewards/{rewardId}");
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

        public async Task<List<PointHistoryDto>> GetPointHistoryAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/points?customerId={customerId}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PointHistoryDto>>() ?? new();
                }
                return new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching point history: {ex.Message}", ex);
            }
        }
    }
}
