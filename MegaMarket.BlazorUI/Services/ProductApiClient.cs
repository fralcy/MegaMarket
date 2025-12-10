using System.Net.Http.Json;
using System.Text.Json;

namespace MegaMarket.BlazorUI.Services;

public class ProductApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);

    public ProductApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync()
    {
        var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/products", _serializerOptions);
        return products ?? [];
    }

    public async Task<ApiResponse<ProductDto>> CreateAsync(ProductCreateUpdateDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/products", dto, _serializerOptions);
        return await ToApiResponse<ProductDto>(response);
    }

    public async Task<ApiResponse> UpdateAsync(int id, ProductCreateUpdateDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", dto, _serializerOptions);
        return await ToApiResponse(response);
    }

    public async Task<ApiResponse> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/products/{id}");
        return await ToApiResponse(response);
    }

    private async Task<ApiResponse> ToApiResponse(HttpResponseMessage response)
    {
        var error = response.IsSuccessStatusCode ? null : await response.Content.ReadAsStringAsync();
        return new ApiResponse(response.IsSuccessStatusCode, error);
    }

    private async Task<ApiResponse<T>> ToApiResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>(_serializerOptions);
            return new ApiResponse<T>(true, data, null);
        }

        var error = await response.Content.ReadAsStringAsync();
        return new ApiResponse<T>(false, default, error);
    }
}

public record ProductDto
{
    public int ProductId { get; init; }
    public string Barcode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Category { get; init; }
    public decimal UnitPrice { get; init; }
    public int QuantityInStock { get; init; }
    public int MinQuantity { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public bool IsPerishable { get; init; }
}

public record ProductCreateUpdateDto
{
    public string Barcode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Category { get; init; }
    public decimal UnitPrice { get; init; }
    public int QuantityInStock { get; init; }
    public int MinQuantity { get; init; }
    public DateTime? ExpiryDate { get; init; }
    public bool IsPerishable { get; init; }
}

public record ApiResponse(bool Success, string? Error);
public record ApiResponse<T>(bool Success, T? Data, string? Error);
