using System.ComponentModel.DataAnnotations;

namespace MegaMarket.API.DTOs.Imports;

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

public record ImportLineCreateDto
{
    public int? ProductId { get; init; }

    [Required, StringLength(200)]
    public string ProductName { get; init; } = string.Empty;

    [StringLength(100)]
    public string? Barcode { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    [Range(0, 999999)]
    public decimal UnitPrice { get; init; }

    public DateTime? ExpiryDate { get; init; }
}

public record ImportCreateDto
{
    [Required, StringLength(200)]
    public string Supplier { get; init; } = string.Empty;

    [Required]
    public DateTime ImportDate { get; init; } = DateTime.Today;

    [Required, StringLength(20)]
    public string Status { get; init; } = "Draft";

    [Required, StringLength(100)]
    public string Staff { get; init; } = "Admin User";

    [Required]
    public List<ImportLineCreateDto> Items { get; init; } = new();
}

public record ImportUpdateDto : ImportCreateDto
{
    public int ImportId { get; init; }
}
