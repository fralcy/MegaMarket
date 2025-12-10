using Microsoft.EntityFrameworkCore;
using MegaMarket.API.DTOs.Imports;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.Data.Data;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Services.Implementations;

public class ImportService : IImportService
{
    private readonly MegaMarketDbContext _dbContext;

    public ImportService(MegaMarketDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Import>> GetAllAsync()
    {
        return await _dbContext.Imports
            .AsNoTracking()
            .Include(i => i.Supplier)
            .Include(i => i.User)
            .Include(i => i.ImportDetails)
                .ThenInclude(id => id.Product)
            .OrderByDescending(i => i.ImportDate)
            .ToListAsync();
    }

    public async Task<Import?> GetByIdAsync(int id)
    {
        return await _dbContext.Imports
            .AsNoTracking()
            .Include(i => i.Supplier)
            .Include(i => i.User)
            .Include(i => i.ImportDetails)
                .ThenInclude(id => id.Product)
            .FirstOrDefaultAsync(i => i.ImportId == id);
    }

    public async Task<Import> CreateAsync(ImportCreateDto dto)
    {
        if (dto.Items is null || !dto.Items.Any())
        {
            throw new InvalidOperationException("At least one line item is required.");
        }

        var supplier = await GetOrCreateSupplierAsync(dto.Supplier);
        var user = await GetOrCreateUserAsync(dto.Staff);

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        var import = new Import
        {
            SupplierId = supplier.SupplierId,
            UserId = user.UserId,
            ImportDate = dto.ImportDate.Date,
            Status = dto.Status
        };

        foreach (var line in dto.Items)
        {
            var product = await ResolveProductAsync(line);

            var detail = new ImportDetail
            {
                ProductId = product.ProductId,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                ExpiryDate = line.ExpiryDate?.Date
            };

            import.ImportDetails.Add(detail);

            product.QuantityInStock += line.Quantity;
            product.UnitPrice = line.UnitPrice;
            product.ExpiryDate = line.ExpiryDate?.Date;
            product.IsPerishable = product.IsPerishable || line.ExpiryDate.HasValue;
        }

        import.TotalCost = import.ImportDetails.Sum(d => d.Quantity * d.UnitPrice);

        _dbContext.Imports.Add(import);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        await _dbContext.Entry(import).Reference(i => i.Supplier).LoadAsync();
        await _dbContext.Entry(import).Reference(i => i.User).LoadAsync();
        await _dbContext.Entry(import).Collection(i => i.ImportDetails).LoadAsync();
        await _dbContext.Entry(import).Collection(i => i.ImportDetails).Query().Include(id => id.Product).LoadAsync();

        return import;
    }

    public async Task<Import?> UpdateAsync(ImportUpdateDto dto)
    {
        var import = await _dbContext.Imports
            .Include(i => i.ImportDetails)
            .FirstOrDefaultAsync(i => i.ImportId == dto.ImportId);

        if (import is null)
        {
            return null;
        }

        if (dto.Items is null || !dto.Items.Any())
        {
            throw new InvalidOperationException("At least one line item is required.");
        }

        var supplier = await GetOrCreateSupplierAsync(dto.Supplier);
        var user = await GetOrCreateUserAsync(dto.Staff);

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        import.SupplierId = supplier.SupplierId;
        import.UserId = user.UserId;
        import.ImportDate = dto.ImportDate.Date;
        import.Status = dto.Status;
        import.TotalCost = 0;

        _dbContext.ImportDetails.RemoveRange(import.ImportDetails);
        import.ImportDetails.Clear();

        foreach (var line in dto.Items)
        {
            var product = await ResolveProductAsync(line);
            var detail = new ImportDetail
            {
                ImportId = import.ImportId,
                ProductId = product.ProductId,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                ExpiryDate = line.ExpiryDate?.Date
            };

            import.ImportDetails.Add(detail);
        }

        import.TotalCost = import.ImportDetails.Sum(d => d.Quantity * d.UnitPrice);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        await _dbContext.Entry(import).Collection(i => i.ImportDetails).Query().Include(id => id.Product).LoadAsync();
        await _dbContext.Entry(import).Reference(i => i.Supplier).LoadAsync();
        await _dbContext.Entry(import).Reference(i => i.User).LoadAsync();
        return import;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var import = await _dbContext.Imports.FirstOrDefaultAsync(i => i.ImportId == id);
        if (import is null)
        {
            return false;
        }

        _dbContext.Imports.Remove(import);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<Product> ResolveProductAsync(ImportLineCreateDto line)
    {
        Product? product = null;

        if (line.ProductId.HasValue)
        {
            product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == line.ProductId.Value);
            if (product is null)
            {
                throw new InvalidOperationException($"Product {line.ProductId.Value} not found.");
            }
        }
        else if (!string.IsNullOrWhiteSpace(line.Barcode))
        {
            var barcode = line.Barcode.Trim();
            product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        if (product is not null)
        {
            return product;
        }

        var newBarcode = string.IsNullOrWhiteSpace(line.Barcode)
            ? $"NEW-{DateTime.UtcNow.Ticks % 1_000_000:D6}"
            : line.Barcode.Trim();

        product = new Product
        {
            Barcode = newBarcode,
            Name = line.ProductName.Trim(),
            UnitPrice = line.UnitPrice,
            QuantityInStock = 0,
            MinQuantity = 0,
            ExpiryDate = line.ExpiryDate?.Date,
            IsPerishable = line.ExpiryDate.HasValue
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return product;
    }

    private async Task<Supplier> GetOrCreateSupplierAsync(string supplierName)
    {
        var name = supplierName.Trim();
        var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(s => s.Name == name);
        if (supplier is not null)
        {
            return supplier;
        }

        supplier = new Supplier { Name = name };
        _dbContext.Suppliers.Add(supplier);
        await _dbContext.SaveChangesAsync();
        return supplier;
    }

    private async Task<User> GetOrCreateUserAsync(string staffName)
    {
        var name = staffName.Trim();
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.FullName == name);
        if (user is not null)
        {
            return user;
        }

        user = new User
        {
            FullName = name,
            Username = name.Replace(" ", ".").ToLowerInvariant(),
            Password = "Password123!",
            Role = "Admin"
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
}
