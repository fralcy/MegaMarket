using System.Net.Http.Json;
using System.Text.Json;
using MegaMarket.BlazorUI.Models.Dashboard;
using MegaMarket.BlazorUI.Services.Auth;

namespace MegaMarket.BlazorUI.Services.Dashboard;

public class DashboardApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public DashboardApiClient(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
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

    // ==================== SALES DASHBOARD ====================
    public async Task<SalesDashboardDto?> GetSalesDashboardAsync(DateRangeEnum dateRange)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.GetFromJsonAsync<SalesDashboardDto>(
                $"api/SalesDashboard?dateRange={dateRange}", _serializerOptions);
            return response;
        }
        catch
        {
            return null;
        }
    }

    // ==================== INVENTORY DASHBOARD ====================
    public async Task<InventoryDashboardDto?> GetInventoryDashboardAsync()
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.GetFromJsonAsync<InventoryDashboardDto>(
                "api/InventoryDashboard", _serializerOptions);
            return response;
        }
        catch
        {
            return null;
        }
    }

    // ==================== CUSTOMER DASHBOARD ====================
    public async Task<CustomerDashboardDto?> GetCustomerDashboardAsync(DateRangeEnum dateRange)
    {
        try
        {
            await AddAuthorizationHeader();
            var response = await _httpClient.GetFromJsonAsync<CustomerDashboardDto>(
                $"api/CustomerDashboard?dateRange={dateRange}", _serializerOptions);
            return response;
        }
        catch
        {
            return null;
        }
    }
}
