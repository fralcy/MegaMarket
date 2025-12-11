using System.Net.Http.Json;

namespace MegaMarket.BlazorUI.Services.Imports;

public class ImportService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public ImportService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var baseUrl = configuration["ApiSettings:BaseUrl"] ?? configuration["ApiBaseUrl"] ?? "https://localhost:7284";
        _apiBaseUrl = baseUrl.TrimEnd('/');
    }

    public async Task<List<ImportSummaryDto>> GetImportsAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<ImportSummaryDto>>($"{_apiBaseUrl}/api/imports");
        return result ?? new List<ImportSummaryDto>();
    }

    public async Task<ImportDetailDto?> GetImportAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<ImportDetailDto>($"{_apiBaseUrl}/api/imports/{id}");
    }

    public async Task<ImportDetailDto> CreateImportAsync(ImportCreateRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/imports", request);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ImportDetailDto>() ?? new ImportDetailDto();
        }

        var details = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Unable to save import: {details}");
    }

    public async Task<ImportDetailDto> UpdateImportAsync(int id, ImportCreateRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/imports/{id}", new ImportUpdateRequest(id, request));
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ImportDetailDto>() ?? new ImportDetailDto();
        }

        var details = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Unable to update import: {details}");
    }

    public async Task DeleteImportAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/imports/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var details = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Unable to delete import: {details}");
        }
    }
}

public record ImportSummaryDto
{
    public int ImportId { get; init; }
    public DateTime ImportDate { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public string Staff { get; init; } = string.Empty;
    public decimal TotalCost { get; init; }
    public string Status { get; init; } = "Draft";
    public bool NearExpiry { get; init; }
    public bool Expired { get; init; }
    public int ItemCount { get; init; }
}

public record ImportDetailDto : ImportSummaryDto
{
    public List<ImportLineDto> Items { get; init; } = new();
}

public record ImportLineDto
{
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public string? Category { get; init; }
    public bool IsPerishable { get; init; }
    public decimal LineTotal => Quantity * UnitPrice;
}

public record ImportLineCreateRequest
{
    public int? ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public DateTime? ExpiryDate { get; init; }
}

public record ImportCreateRequest
{
    public string Supplier { get; init; } = string.Empty;
    public DateTime ImportDate { get; init; } = DateTime.Today;
    public string Status { get; init; } = "Draft";
    public string Staff { get; init; } = "Admin User";
    public List<ImportLineCreateRequest> Items { get; init; } = new();
}

public record ImportUpdateRequest
{
    public int ImportId { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public DateTime ImportDate { get; init; } = DateTime.Today;
    public string Status { get; init; } = "Draft";
    public string Staff { get; init; } = "Admin User";
    public List<ImportLineCreateRequest> Items { get; init; } = new();

    public ImportUpdateRequest(int importId, ImportCreateRequest request)
    {
        ImportId = importId;
        Supplier = request.Supplier;
        ImportDate = request.ImportDate;
        Status = request.Status;
        Staff = request.Staff;
        Items = request.Items;
    }
}
