using MegaMarket.API.DTOs.Promotion;
using MegaMarket.BlazorUI.Services.Auth;

namespace MegaMarket.BlazorUI.Services.Promotion
{
    public class PromotionApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly AuthService _authService;

        public PromotionApiClient(HttpClient httpClient, IConfiguration configuration, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? configuration["ApiBaseUrl"] ?? "https://localhost:7284";
            _apiBaseUrl = baseUrl.TrimEnd('/');
        }

        private async Task AddAuthorizationHeader()
        {
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        public async Task<List<PromotionResDto>> GetAllPromotions()
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.GetFromJsonAsync<List<PromotionResDto>>($"{_apiBaseUrl}/api/Promotion");
            return response ?? new List<PromotionResDto>();
        }
        public async Task<PromotionResDto> CreatePromotion(PromotionReqDto promotion)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/Promotion", promotion);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PromotionResDto>() ?? new PromotionResDto();
            }
            var details = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Save failed: {details}");
        }
        public async Task<PromotionResDto> UpdatePromotion(int id, PromotionReqDto promotion)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/Promotion/{id}", promotion);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PromotionResDto>() ?? new PromotionResDto();
            }
            var details = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Update failed: {details}");
        }
        public async Task DeletePromotion(int id)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/Promotion/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var details = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Delete failed: {details}");
            }
        }
    }
}
