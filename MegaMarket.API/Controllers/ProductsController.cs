using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MegaMarket.API.DTOs.Products;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllAsync();
        var result = products.Select(MapToDto);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(product));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductCreateUpdateDto dto)
    {
        try
        {
            var product = await _productService.CreateAsync(MapToEntity(dto));
            var result = MapToDto(product);
            return CreatedAtAction(nameof(GetProduct), new { id = result.ProductId }, result);
        }
        catch (DbUpdateException ex)
        {
            return Conflict($"Could not create product: {ex.GetBaseException().Message}");
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, [FromBody] ProductCreateUpdateDto dto)
    {
        try
        {
            var updated = await _productService.UpdateAsync(MapToEntity(dto, id));
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return Conflict($"Could not update product: {ex.GetBaseException().Message}");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        try
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return Conflict($"Could not delete product: {ex.GetBaseException().Message}");
        }
    }

    private static ProductDto MapToDto(Product product) =>
        new()
        {
            ProductId = product.ProductId,
            Barcode = product.Barcode,
            Name = product.Name,
            Category = product.Category,
            UnitPrice = product.UnitPrice,
            QuantityInStock = product.QuantityInStock,
            MinQuantity = product.MinQuantity,
            ExpiryDate = product.ExpiryDate,
            IsPerishable = product.IsPerishable
        };

    private static Product MapToEntity(ProductCreateUpdateDto dto, int? id = null) =>
        new()
        {
            ProductId = id ?? 0,
            Barcode = dto.Barcode,
            Name = dto.Name,
            Category = dto.Category,
            UnitPrice = dto.UnitPrice,
            QuantityInStock = dto.QuantityInStock,
            MinQuantity = dto.MinQuantity,
            ExpiryDate = dto.ExpiryDate,
            IsPerishable = dto.IsPerishable
        };
}
