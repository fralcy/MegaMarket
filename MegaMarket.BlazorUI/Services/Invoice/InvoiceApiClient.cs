using MegaMarket.API.DTOs.Invoice;
using MegaMarket.BlazorUI.Services.Auth;

namespace MegaMarket.BlazorUI.Services.Invoice
{
    public class InvoiceApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly AuthService _authService;

        public InvoiceApiClient(HttpClient httpClient, IConfiguration configuration, AuthService authService)
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

        public async Task<List<InvoiceResDto>> GetAllInvoices()
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.GetFromJsonAsync<List<InvoiceResDto>>($"{_apiBaseUrl}/api/Invoice");
            return response ?? new List<InvoiceResDto>();
        }

        public async Task<InvoiceResDto> SaveInvoice(InvoiceReqDto invoice)
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/Invoice", invoice);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<InvoiceResDto>() ?? new InvoiceResDto();
            }
            var details = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Save failed: {details}");
        }
    }
}
