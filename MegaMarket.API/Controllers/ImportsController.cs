using Microsoft.AspNetCore.Mvc;
using MegaMarket.API.DTOs.Imports;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportsController : ControllerBase
{
    private readonly IImportService _importService;

    public ImportsController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ImportSummaryDto>>> GetImports()
    {
        var imports = await _importService.GetAllAsync();
        var result = imports.Select(MapToSummary);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ImportDetailDto>> GetImport(int id)
    {
        var import = await _importService.GetByIdAsync(id);
        if (import is null)
        {
            return NotFound();
        }

        return Ok(MapToDetail(import));
    }

    [HttpPost]
    public async Task<ActionResult<ImportDetailDto>> CreateImport([FromBody] ImportCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var import = await _importService.CreateAsync(dto);
            var result = MapToDetail(import);
            return CreatedAtAction(nameof(GetImport), new { id = result.ImportId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ImportDetailDto>> UpdateImport(int id, [FromBody] ImportUpdateDto dto)
    {
        if (id != dto.ImportId)
        {
            return BadRequest("Mismatched import id.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var updated = await _importService.UpdateAsync(dto);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(MapToDetail(updated));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteImport(int id)
    {
        var deleted = await _importService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private static ImportSummaryDto MapToSummary(Import import)
    {
        var expiryFlags = GetExpiryFlags(import);
        return new ImportSummaryDto
        {
            ImportId = import.ImportId,
            ImportDate = import.ImportDate,
            Supplier = import.Supplier?.Name ?? "Unknown",
            Staff = import.User?.FullName ?? "Unknown",
            TotalCost = import.TotalCost,
            Status = import.Status,
            NearExpiry = expiryFlags.nearExpiry,
            Expired = expiryFlags.expired,
            ItemCount = import.ImportDetails.Count
        };
    }

    private static ImportDetailDto MapToDetail(Import import)
    {
        var summary = MapToSummary(import);
        var items = import.ImportDetails
            .Select(id => new ImportLineDto
            {
                ProductId = id.ProductId,
                ProductName = id.Product?.Name ?? "Unknown",
                Barcode = id.Product?.Barcode ?? string.Empty,
                Quantity = id.Quantity,
                UnitPrice = id.UnitPrice,
                ExpiryDate = id.ExpiryDate,
                Category = id.Product?.Category,
                IsPerishable = id.Product?.IsPerishable ?? false
            })
            .ToList();

        return new ImportDetailDto
        {
            ImportId = summary.ImportId,
            ImportDate = summary.ImportDate,
            Supplier = summary.Supplier,
            Staff = summary.Staff,
            TotalCost = summary.TotalCost,
            Status = summary.Status,
            NearExpiry = summary.NearExpiry,
            Expired = summary.Expired,
            ItemCount = summary.ItemCount,
            Items = items
        };
    }

    private static (bool expired, bool nearExpiry) GetExpiryFlags(Import import)
    {
        var expiries = import.ImportDetails
            .Where(id => id.ExpiryDate.HasValue)
            .Select(id => (id.ExpiryDate!.Value.Date - DateTime.Today).TotalDays)
            .ToList();

        var expired = expiries.Any(days => days < 0);
        var nearExpiry = expiries.Any(days => days >= 0 && days <= 7);

        return (expired, nearExpiry);
    }
}
