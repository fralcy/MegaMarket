using System.Net.Http.Json;
using System.Text.Json;

namespace MegaMarket.BlazorUI.Services.CustomerLoyalty
{
    public class TopCustomerDto
    {
        public int CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public int Points { get; set; }
        public string? Rank { get; set; }
    }

    public class TopRewardDto
    {
        public int RewardId { get; set; }
        public string? RewardName { get; set; }
        public string? RewardType { get; set; }
        public int TotalRedeemed { get; set; }
    }

    public class PointsSummaryDto
    {
        public string? TransactionType { get; set; }
        public int TotalPoints { get; set; }
    }

    public class ReportService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private string _apiBaseUrl;

        public ReportService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _apiBaseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:7224";
        }

        public async Task<List<TopCustomerDto>> GetTopCustomersAsync(int limit = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/reports/customers/top?limit={limit}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        var dataElement = doc.RootElement.GetProperty("data");
                        return JsonSerializer.Deserialize<List<TopCustomerDto>>(dataElement.GetRawText()) ?? new();
                    }
                }
                return new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching top customers: {ex.Message}", ex);
            }
        }

        public async Task<List<TopRewardDto>> GetTopRewardsAsync(int limit = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/reports/rewards/top?limit={limit}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        var dataElement = doc.RootElement.GetProperty("data");
                        return JsonSerializer.Deserialize<List<TopRewardDto>>(dataElement.GetRawText()) ?? new();
                    }
                }
                return new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching top rewards: {ex.Message}", ex);
            }
        }

        public async Task<List<PointsSummaryDto>> GetPointsSummaryAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/reports/points/summary");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        var dataElement = doc.RootElement.GetProperty("data");
                        return JsonSerializer.Deserialize<List<PointsSummaryDto>>(dataElement.GetRawText()) ?? new();
                    }
                }
                return new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching points summary: {ex.Message}", ex);
            }
        }
    }
}
