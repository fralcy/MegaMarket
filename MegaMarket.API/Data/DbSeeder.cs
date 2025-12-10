using Microsoft.EntityFrameworkCore;
using MegaMarket.Data.Data;
using MegaMarket.Data.Models;

namespace MegaMarket.API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(MegaMarketDbContext dbContext)
    {
        await dbContext.Database.MigrateAsync();

        await SeedUsersAsync(dbContext);
        await SeedSuppliersAsync(dbContext);
        await SeedProductsAsync(dbContext);
        await SeedImportsAsync(dbContext);
    }

    private static async Task SeedUsersAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var users = new[]
        {
            new User { FullName = "Admin User", Username = "admin", Password = "Password123!", Role = "Admin" },
            new User { FullName = "John Manager", Username = "john.manager", Password = "Password123!", Role = "Manager" }
        };

        dbContext.Users.AddRange(users);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedSuppliersAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Suppliers.AnyAsync())
        {
            return;
        }

        var suppliers = new[]
        {
            new Supplier { Name = "Fresh Foods Co.", Phone = "555-1200" },
            new Supplier { Name = "Dairy Direct", Phone = "555-8831" },
            new Supplier { Name = "Global Groceries", Phone = "555-7412" },
            new Supplier { Name = "Premium Produce", Phone = "555-9102" }
        };

        dbContext.Suppliers.AddRange(suppliers);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Products.AnyAsync())
        {
            return;
        }

        var products = new[]
        {
            new Product
            {
                Barcode = "1110001110001",
                Name = "Organic Apples 1kg",
                Category = "Produce",
                UnitLabel = "Bag",
                UnitPrice = 45000,
                QuantityInStock = 120,
                MinQuantity = 30,
                IsPerishable = true,
                ExpiryDate = DateTime.Today.AddDays(10)
            },
            new Product
            {
                Barcode = "2220002220002",
                Name = "Whole Milk 1L",
                Category = "Dairy",
                UnitLabel = "Bottle",
                UnitPrice = 18000,
                QuantityInStock = 220,
                MinQuantity = 60,
                IsPerishable = true,
                ExpiryDate = DateTime.Today.AddDays(5)
            },
            new Product
            {
                Barcode = "3330003330003",
                Name = "Free Range Eggs 12ct",
                Category = "Dairy",
                UnitLabel = "Carton",
                UnitPrice = 52000,
                QuantityInStock = 150,
                MinQuantity = 40,
                IsPerishable = true,
                ExpiryDate = DateTime.Today.AddDays(2)
            },
            new Product
            {
                Barcode = "4440004440004",
                Name = "Roasted Coffee Beans 500g",
                Category = "Beverages",
                UnitLabel = "Bag",
                UnitPrice = 95000,
                QuantityInStock = 80,
                MinQuantity = 20,
                IsPerishable = false
            },
            new Product
            {
                Barcode = "5550005550005",
                Name = "Bottled Water 24x500ml",
                Category = "Beverages",
                UnitLabel = "Pack",
                UnitPrice = 120000,
                QuantityInStock = 60,
                MinQuantity = 15,
                IsPerishable = false
            }
        };

        dbContext.Products.AddRange(products);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedImportsAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Imports.AnyAsync())
        {
            return;
        }

        var users = await dbContext.Users.ToListAsync();
        var suppliers = await dbContext.Suppliers.ToListAsync();
        var products = await dbContext.Products.ToListAsync();

        if (!users.Any() || !suppliers.Any() || !products.Any())
        {
            return;
        }

        var admin = users.First();

        Import BuildImport(string supplierName, DateTime importDate, string status, params (string barcode, int qty, decimal price, int expiryOffset)[] items)
        {
            var supplier = suppliers.First(s => s.Name == supplierName);
            var import = new Import
            {
                SupplierId = supplier.SupplierId,
                UserId = admin.UserId,
                ImportDate = importDate,
                Status = status
            };

            foreach (var (barcode, qty, price, expiryOffset) in items)
            {
                var product = products.First(p => p.Barcode == barcode);
                import.ImportDetails.Add(new ImportDetail
                {
                    ProductId = product.ProductId,
                    Quantity = qty,
                    UnitPrice = price,
                    ExpiryDate = expiryOffset == 0 ? null : DateTime.Today.AddDays(expiryOffset)
                });
                product.QuantityInStock += qty;
                product.UnitPrice = price;
                if (expiryOffset != 0)
                {
                    product.IsPerishable = true;
                    product.ExpiryDate = DateTime.Today.AddDays(expiryOffset);
                }
            }

            import.TotalCost = import.ImportDetails.Sum(d => d.Quantity * d.UnitPrice);
            return import;
        }

        var imports = new[]
        {
            BuildImport("Fresh Foods Co.", DateTime.Today.AddDays(-8), "Completed",
                ("1110001110001", 80, 42000, 12),
                ("3330003330003", 60, 50000, 5)),
            BuildImport("Dairy Direct", DateTime.Today.AddDays(-3), "Completed",
                ("2220002220002", 120, 17000, 6)),
            BuildImport("Global Groceries", DateTime.Today.AddDays(-1), "Draft",
                ("4440004440004", 50, 90000, 0),
                ("5550005550005", 40, 115000, 0)),
            BuildImport("Premium Produce", DateTime.Today.AddDays(-15), "Cancelled",
                ("1110001110001", 40, 43000, -1))
        };

        dbContext.Imports.AddRange(imports);
        await dbContext.SaveChangesAsync();
    }
}
