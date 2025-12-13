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
        await SeedShiftTypesAsync(dbContext);
        await SeedAttendancesAsync(dbContext);
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
            // Quản trị hệ thống (System Admin)
            new User { FullName = "Admin User", Username = "admin", Password = BCrypt.Net.BCrypt.HashPassword("Password123!"), Role = "Admin", Email = "admin@megamarket.com", Phone = "0901234567" },
            
            // Quản lý cửa hàng (Manager)
            new User { FullName = "John Manager", Username = "john.manager", Password = BCrypt.Net.BCrypt.HashPassword("Password123!"), Role = "Manager", Email = "john@megamarket.com", Phone = "0907654321" },
            
            // Thu ngân/POS (Cashier)
            new User { FullName = "Sarah Cashier", Username = "sarah.cashier", Password = BCrypt.Net.BCrypt.HashPassword("Password123!"), Role = "Cashier", Email = "sarah@megamarket.com", Phone = "0912345678" },
            
            // Nhân viên kho (Warehouse)
            new User { FullName = "Mike Warehouse", Username = "mike.warehouse", Password = BCrypt.Net.BCrypt.HashPassword("Password123!"), Role = "Warehouse", Email = "mike@megamarket.com", Phone = "0923456789" }
        };

        dbContext.Users.AddRange(users);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedShiftTypesAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.ShiftTypes.AnyAsync())
        {
            return;
        }

        var shiftTypes = new[]
        {
            new ShiftType
            {
                Name = "Morning Shift",
                StartTime = new TimeSpan(6, 0, 0),   // 6:00 AM
                EndTime = new TimeSpan(14, 0, 0),    // 2:00 PM
                WagePerHour = 40000
            },
            new ShiftType
            {
                Name = "Afternoon Shift",
                StartTime = new TimeSpan(14, 0, 0),  // 2:00 PM
                EndTime = new TimeSpan(22, 0, 0),    // 10:00 PM
                WagePerHour = 45000
            },
            new ShiftType
            {
                Name = "Night Shift",
                StartTime = new TimeSpan(22, 0, 0),  // 10:00 PM
                EndTime = new TimeSpan(6, 0, 0),     // 6:00 AM
                WagePerHour = 55000
            },
            new ShiftType
            {
                Name = "Full Day",
                StartTime = new TimeSpan(8, 0, 0),   // 8:00 AM
                EndTime = new TimeSpan(17, 0, 0),    // 5:00 PM
                WagePerHour = 42000
            }
        };

        dbContext.ShiftTypes.AddRange(shiftTypes);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedAttendancesAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Attendances.AnyAsync())
        {
            return;
        }

        var users = await dbContext.Users.ToListAsync();
        var shiftTypes = await dbContext.ShiftTypes.ToListAsync();

        if (!users.Any() || !shiftTypes.Any())
        {
            return;
        }

        var admin = users.FirstOrDefault(u => u.Role == "Admin");
        var manager = users.FirstOrDefault(u => u.Role == "Manager");
        var cashier = users.FirstOrDefault(u => u.Role == "Cashier");
        var warehouse = users.FirstOrDefault(u => u.Role == "Warehouse");

        var morningShift = shiftTypes.FirstOrDefault(s => s.Name == "Morning Shift");
        var afternoonShift = shiftTypes.FirstOrDefault(s => s.Name == "Afternoon Shift");
        var fullDayShift = shiftTypes.FirstOrDefault(s => s.Name == "Full Day");

        if (admin == null || morningShift == null || afternoonShift == null || fullDayShift == null)
        {
            return;
        }

        var attendances = new List<Attendance>();

        // Admin - Last 7 days
        for (int i = 7; i >= 1; i--)
        {
            var date = DateTime.Today.AddDays(-i);
            attendances.Add(new Attendance
            {
                UserId = admin.UserId,
                ShiftTypeId = fullDayShift.ShiftTypeId,
                Date = date,
                CheckIn = date.AddHours(8).AddMinutes(i % 2 == 0 ? 0 : 5),
                CheckOut = date.AddHours(17).AddMinutes(i % 2 == 0 ? 0 : 10),
                IsLate = i % 2 != 0,
                Note = i % 2 == 0 ? "On time" : "Late check-in"
            });
        }

        // Manager - Last 5 days
        if (manager != null)
        {
            for (int i = 5; i >= 1; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                attendances.Add(new Attendance
                {
                    UserId = manager.UserId,
                    ShiftTypeId = morningShift.ShiftTypeId,
                    Date = date,
                    CheckIn = date.AddHours(6),
                    CheckOut = date.AddHours(14),
                    IsLate = false,
                    Note = "Regular shift"
                });
            }
        }

        // Cashier - Last 3 days
        if (cashier != null)
        {
            for (int i = 3; i >= 1; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                attendances.Add(new Attendance
                {
                    UserId = cashier.UserId,
                    ShiftTypeId = afternoonShift.ShiftTypeId,
                    Date = date,
                    CheckIn = date.AddHours(14).AddMinutes(i % 2 == 0 ? 0 : 5),
                    CheckOut = date.AddHours(22),
                    IsLate = i % 2 != 0,
                    Note = i % 2 == 0 ? "On time" : "Traffic delay"
                });
            }
        }

        // Warehouse - Last 4 days
        if (warehouse != null)
        {
            for (int i = 4; i >= 1; i--)
            {
                var date = DateTime.Today.AddDays(-i);
                attendances.Add(new Attendance
                {
                    UserId = warehouse.UserId,
                    ShiftTypeId = fullDayShift.ShiftTypeId,
                    Date = date,
                    CheckIn = date.AddHours(8),
                    CheckOut = date.AddHours(17),
                    IsLate = false,
                    Note = "Inventory check"
                });
            }
        }

        dbContext.Attendances.AddRange(attendances);
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
