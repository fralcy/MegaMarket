using Microsoft.EntityFrameworkCore;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.Data.Data;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Services.Implementations;

public class ProductService : IProductService
{
    private readonly MegaMarketDbContext _dbContext;

    public ProductService(MegaMarketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbContext.Products.AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        var existing = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
        if (existing is null)
        {
            return false;
        }

        existing.Barcode = product.Barcode;
        existing.Name = product.Name;
        existing.Category = product.Category;
        existing.UnitLabel = product.UnitLabel;
        existing.UnitPrice = product.UnitPrice;
        existing.OriginalPrice = product.OriginalPrice;
        existing.DiscountPercent = product.DiscountPercent;
        existing.QuantityInStock = product.QuantityInStock;
        existing.MinQuantity = product.MinQuantity;
        existing.ExpiryDate = product.ExpiryDate;
        existing.IsPerishable = product.IsPerishable;
        existing.ImageUrl = product.ImageUrl;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        if (existing is null)
        {
            return false;
        }

        _dbContext.Products.Remove(existing);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
