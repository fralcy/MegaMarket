using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace MegaMarket.BlazorUI.Services.Products;

public class ProductDto
{
    public int ProductId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? UnitLabel { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int? DiscountPercent { get; set; }
    public int QuantityInStock { get; set; }
    public int MinQuantity { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsPerishable { get; set; }
    public string? ImageUrl { get; set; }
}

public class ProductUpsertRequest
{
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? UnitLabel { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal UnitPrice { get; set; }
    public int? DiscountPercent { get; set; }
    public int QuantityInStock { get; set; }
    public int MinQuantity { get; set; }
    public bool IsPerishable { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? ImageUrl { get; set; }
}

public class ProductInputModel : IValidatableObject
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Barcode is required")]
    [StringLength(100)]
    public string Barcode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(50)]
    public string? UnitLabel { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Original price must be >= 0")]
    public decimal? OriginalPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Unit price must be >= 0")]
    public decimal UnitPrice { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
    public int? DiscountPercent { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be >= 0")]
    public int QuantityInStock { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Min quantity must be >= 0")]
    public int MinQuantity { get; set; }

    public bool IsPerishable { get; set; }

    public DateTime? ExpiryDate { get; set; }

    [StringLength(500)]
    [Url(ErrorMessage = "Image URL must be a valid URL")]
    public string? ImageUrl { get; set; }

    public ProductUpsertRequest ToRequest() => new()
    {
        Barcode = Barcode.Trim(),
        Name = Name.Trim(),
        Category = string.IsNullOrWhiteSpace(Category) ? null : Category.Trim(),
        UnitLabel = string.IsNullOrWhiteSpace(UnitLabel) ? null : UnitLabel.Trim(),
        OriginalPrice = OriginalPrice,
        UnitPrice = UnitPrice,
        DiscountPercent = DiscountPercent,
        QuantityInStock = QuantityInStock,
        MinQuantity = MinQuantity,
        IsPerishable = IsPerishable,
        ExpiryDate = ExpiryDate?.Date,
        ImageUrl = string.IsNullOrWhiteSpace(ImageUrl) ? null : ImageUrl.Trim()
    };

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IsPerishable)
        {
            if (!ExpiryDate.HasValue)
            {
                yield return new ValidationResult("Expiry date is required for perishable items", new[] { nameof(ExpiryDate) });
            }
            else if (ExpiryDate.Value.Date < DateTime.Today)
            {
                yield return new ValidationResult("Expiry date must be today or later", new[] { nameof(ExpiryDate) });
            }
        }
    }
}

public class ProductService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public ProductService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var baseUrl = configuration["ApiSettings:BaseUrl"] ??
                      configuration["ApiBaseUrl"] ??
                      "https://localhost:7284";
        _apiBaseUrl = baseUrl.TrimEnd('/');
    }

    public async Task<List<ProductDto>> GetProductsAsync()
    {
        try
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>($"{_apiBaseUrl}/api/products");
            return products ?? new List<ProductDto>();
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("Unable to load products right now. Please try again.", ex);
        }
    }

    public async Task<ProductDto?> GetProductAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProductDto>($"{_apiBaseUrl}/api/products/{id}");
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Unable to load product {id}.", ex);
        }
    }

    public async Task<ProductDto> CreateProductAsync(ProductInputModel product)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/products", product.ToRequest());

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ProductDto>() ?? new ProductDto();
        }

        var details = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Create failed: {details}");
    }

    public async Task<ProductDto> UpdateProductAsync(int id, ProductInputModel product)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/products/{id}", product.ToRequest());

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent ||
                response.Content.Headers.ContentLength == 0)
            {
                return MapInputToDto(id, product);
            }

            return await response.Content.ReadFromJsonAsync<ProductDto>() ?? MapInputToDto(id, product);
        }

        var details = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Update failed: {details}");
    }

    public async Task DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/products/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var details = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Delete failed: {details}");
        }
    }

    private static ProductDto MapInputToDto(int id, ProductInputModel product) => new()
    {
        ProductId = id,
        Barcode = product.Barcode,
        Name = product.Name,
        Category = product.Category,
        UnitLabel = product.UnitLabel,
        UnitPrice = product.UnitPrice,
        OriginalPrice = product.OriginalPrice,
        DiscountPercent = product.DiscountPercent,
        QuantityInStock = product.QuantityInStock,
        MinQuantity = product.MinQuantity,
        ExpiryDate = product.ExpiryDate,
        IsPerishable = product.IsPerishable,
        ImageUrl = product.ImageUrl
    };
}
