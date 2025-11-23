using System.ComponentModel.DataAnnotations;

namespace MegaMarket.API.DTOs.Products;

public record ProductDto
{
    public int ProductId { get; init; }

    [Required, StringLength(100)]
    public string Barcode { get; init; } = string.Empty;

    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; init; }

    [Range(0, 999999)]
    public decimal UnitPrice { get; init; }

    [Range(0, int.MaxValue)]
    public int QuantityInStock { get; init; }

    [Range(0, int.MaxValue)]
    public int MinQuantity { get; init; }

    public DateTime? ExpiryDate { get; init; }

    public bool IsPerishable { get; init; }
}

public record ProductCreateUpdateDto
{
    [Required, StringLength(100)]
    public string Barcode { get; init; } = string.Empty;

    [Required, StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; init; }

    [Range(0, 999999)]
    public decimal UnitPrice { get; init; }

    [Range(0, int.MaxValue)]
    public int QuantityInStock { get; init; }

    [Range(0, int.MaxValue)]
    public int MinQuantity { get; init; }

    public DateTime? ExpiryDate { get; init; }

    public bool IsPerishable { get; init; }
}
