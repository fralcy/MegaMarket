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
        await SeedCustomersAsync(dbContext);
        await SeedPromotionsAsync(dbContext);
        await SeedRewardsAsync(dbContext);
        await SeedInvoicesAsync(dbContext);
        await SeedOrderRequestsAsync(dbContext);
    }

    private static async Task SeedUsersAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        // Fixed BCrypt hash for "Password123!" - matches SQL setup script
        // This ensures consistent data between DbSeeder and SQL file
        const string passwordHash = "$2a$12$dXVEhDlHpyOvZ8hgHfwKJeX3tfKp1KUi.q1eIP43AmSmYhLpdj.w.";

        var users = new[]
        {
            // Quản trị hệ thống (System Admin)
            new User { FullName = "Admin User", Username = "admin", Password = passwordHash, Role = "Admin", Email = "admin@megamarket.com", Phone = "0901234567" },

            // Quản lý cửa hàng (Manager)
            new User { FullName = "John Manager", Username = "john.manager", Password = passwordHash, Role = "Manager", Email = "john@megamarket.com", Phone = "0907654321" },

            // Thu ngân/POS (Cashier)
            new User { FullName = "Sarah Cashier", Username = "sarah.cashier", Password = passwordHash, Role = "Cashier", Email = "sarah@megamarket.com", Phone = "0912345678" },

            // Nhân viên kho (Warehouse)
            new User { FullName = "Mike Warehouse", Username = "mike.warehouse", Password = passwordHash, Role = "Warehouse", Email = "mike@megamarket.com", Phone = "0923456789" }
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

    private static async Task SeedCustomersAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Customers.AnyAsync())
        {
            return;
        }

        var customers = new[]
        {
            new Customer { FullName = "Nguyen Van A", Phone = "0909123456", Email = "nguyenvana@email.com", Points = 1250, Rank = "Gold" },
            new Customer { FullName = "Tran Thi B", Phone = "0918765432", Email = "tranthib@email.com", Points = 850, Rank = "Silver" },
            new Customer { FullName = "Le Van C", Phone = "0987654321", Email = "levanc@email.com", Points = 2500, Rank = "Platinum" },
            new Customer { FullName = "Pham Thi D", Phone = "0976543210", Email = "phamthid@email.com", Points = 450, Rank = "Silver" },
            new Customer { FullName = "Hoang Van E", Phone = "0965432109", Email = "hoangvane@email.com", Points = 150, Rank = "Silver" }
        };

        dbContext.Customers.AddRange(customers);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedPromotionsAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Promotions.AnyAsync())
        {
            return;
        }

        var products = await dbContext.Products.ToListAsync();
        if (!products.Any())
        {
            return;
        }

        var apples = products.FirstOrDefault(p => p.Barcode == "1110001110001");
        var milk = products.FirstOrDefault(p => p.Barcode == "2220002220002");
        var eggs = products.FirstOrDefault(p => p.Barcode == "3330003330003");
        var coffee = products.FirstOrDefault(p => p.Barcode == "4440004440004");

        var promotions = new[]
        {
            new Promotion
            {
                Name = "Summer Sale 2024",
                Description = "Discount for all products in summer",
                DiscountType = "percent",
                DiscountValue = 15,
                StartDate = DateTime.Today.AddDays(-10),
                EndDate = DateTime.Today.AddDays(20),
                Type = "product",
                PromotionProducts = new List<PromotionProduct>
                {
                    new PromotionProduct { ProductId = apples!.ProductId },
                    new PromotionProduct { ProductId = coffee!.ProductId }
                }
            },
            new Promotion
            {
                Name = "New Year Special",
                Description = "Special discount for new year shopping",
                DiscountType = "percent",
                DiscountValue = 20,
                StartDate = DateTime.Today.AddDays(-30),
                EndDate = DateTime.Today.AddDays(-5),
                Type = "invoice"
            },
            new Promotion
            {
                Name = "Weekend Deal",
                Description = "Fixed discount on weekends",
                DiscountType = "fixed",
                DiscountValue = 50000,
                StartDate = DateTime.Today.AddDays(-2),
                EndDate = DateTime.Today.AddDays(5),
                Type = "invoice"
            },
            new Promotion
            {
                Name = "Dairy Products Promo",
                Description = "Special for dairy category",
                DiscountType = "percent",
                DiscountValue = 10,
                StartDate = DateTime.Today.AddDays(-5),
                EndDate = DateTime.Today.AddDays(25),
                Type = "product",
                PromotionProducts = new List<PromotionProduct>
                {
                    new PromotionProduct { ProductId = milk!.ProductId },
                    new PromotionProduct { ProductId = eggs!.ProductId }
                }
            }
        };

        dbContext.Promotions.AddRange(promotions);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedRewardsAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Rewards.AnyAsync())
        {
            return;
        }

        var rewards = new[]
        {
            new Reward { Name = "Free Coffee Voucher", Description = "Get a free coffee bag", PointCost = 500, RewardType = "Voucher", Value = 95000, QuantityAvailable = 50, IsActive = true },
            new Reward { Name = "10% Discount Coupon", Description = "10% off on next purchase", PointCost = 300, RewardType = "Discount", Value = 0, QuantityAvailable = 100, IsActive = true },
            new Reward { Name = "Shopping Bag", Description = "Reusable shopping bag", PointCost = 200, RewardType = "Gift", Value = 0, QuantityAvailable = 75, IsActive = true },
            new Reward { Name = "50k Cash Voucher", Description = "50,000 VND discount voucher", PointCost = 800, RewardType = "Voucher", Value = 50000, QuantityAvailable = 30, IsActive = true },
            new Reward { Name = "Premium Gift Set", Description = "Special gift set for loyal customers", PointCost = 2000, RewardType = "Gift", Value = 0, QuantityAvailable = 20, IsActive = true }
        };

        dbContext.Rewards.AddRange(rewards);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedInvoicesAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.Invoices.AnyAsync())
        {
            return;
        }

        var users = await dbContext.Users.ToListAsync();
        var customers = await dbContext.Customers.ToListAsync();
        var products = await dbContext.Products.ToListAsync();
        var promotions = await dbContext.Promotions.ToListAsync();
        var rewards = await dbContext.Rewards.ToListAsync();

        if (!users.Any() || !customers.Any() || !products.Any())
        {
            return;
        }

        var cashier = users.FirstOrDefault(u => u.Role == "Cashier");
        var customer1 = customers.FirstOrDefault(c => c.FullName == "Nguyen Van A");
        var customer2 = customers.FirstOrDefault(c => c.FullName == "Tran Thi B");
        var customer3 = customers.FirstOrDefault(c => c.FullName == "Le Van C");

        var apples = products.FirstOrDefault(p => p.Barcode == "1110001110001");
        var milk = products.FirstOrDefault(p => p.Barcode == "2220002220002");
        var eggs = products.FirstOrDefault(p => p.Barcode == "3330003330003");
        var coffee = products.FirstOrDefault(p => p.Barcode == "4440004440004");
        var water = products.FirstOrDefault(p => p.Barcode == "5550005550005");

        var weekendDeal = promotions.FirstOrDefault(p => p.Name == "Weekend Deal");
        var discountCoupon = rewards.FirstOrDefault(r => r.Name == "10% Discount Coupon");
        var shoppingBag = rewards.FirstOrDefault(r => r.Name == "Shopping Bag");

        if (cashier == null || customer1 == null || customer2 == null || customer3 == null)
        {
            return;
        }

        // Invoice 1: Customer purchase with promotion
        var invoice1 = new Invoice
        {
            UserId = cashier.UserId,
            CustomerId = customer1.CustomerId,
            CreatedAt = DateTime.Now.AddDays(-2),
            TotalBeforeDiscount = 300000,
            TotalAmount = 250000,
            PaymentMethod = "cash",
            ReceivedAmount = 300000,
            ChangeAmount = 50000,
            Status = "Paid",
            PromotionId = weekendDeal?.PromotionId,
            InvoiceDetails = new List<InvoiceDetail>
            {
                new InvoiceDetail { ProductId = apples!.ProductId, Quantity = 3, UnitPrice = 45000, DiscountPerUnit = 0 },
                new InvoiceDetail { ProductId = milk!.ProductId, Quantity = 5, UnitPrice = 18000, DiscountPerUnit = 0 },
                new InvoiceDetail { ProductId = coffee!.ProductId, Quantity = 1, UnitPrice = 95000, DiscountPerUnit = 0 }
            },
            PointTransactions = new List<PointTransaction>
            {
                new PointTransaction { CustomerId = customer1.CustomerId, PointChange = 50, TransactionType = "Earn", CreatedAt = DateTime.Now.AddDays(-2), Description = "Points earned from purchase" }
            }
        };

        // Invoice 2: Non-customer purchase
        var invoice2 = new Invoice
        {
            UserId = cashier.UserId,
            CustomerId = null,
            CreatedAt = DateTime.Now.AddDays(-1),
            TotalBeforeDiscount = 156000,
            TotalAmount = 156000,
            PaymentMethod = "card",
            ReceivedAmount = 156000,
            ChangeAmount = 0,
            Status = "Paid",
            InvoiceDetails = new List<InvoiceDetail>
            {
                new InvoiceDetail { ProductId = eggs!.ProductId, Quantity = 2, UnitPrice = 52000, DiscountPerUnit = 0 },
                new InvoiceDetail { ProductId = milk.ProductId, Quantity = 2, UnitPrice = 18000, DiscountPerUnit = 0 },
                new InvoiceDetail { ProductId = water!.ProductId, Quantity = 1, UnitPrice = 120000, DiscountPerUnit = 0 }
            }
        };

        // Invoice 3: Customer purchase earning points
        var invoice3 = new Invoice
        {
            UserId = cashier.UserId,
            CustomerId = customer3.CustomerId,
            CreatedAt = DateTime.Now.AddHours(-5),
            TotalBeforeDiscount = 450000,
            TotalAmount = 450000,
            PaymentMethod = "bank_transfer",
            ReceivedAmount = 450000,
            ChangeAmount = 0,
            Status = "Paid",
            InvoiceDetails = new List<InvoiceDetail>
            {
                new InvoiceDetail { ProductId = coffee.ProductId, Quantity = 2, UnitPrice = 95000, DiscountPerUnit = 0 },
                new InvoiceDetail { ProductId = water.ProductId, Quantity = 2, UnitPrice = 120000, DiscountPerUnit = 0 },
                new InvoiceDetail { ProductId = apples.ProductId, Quantity = 1, UnitPrice = 45000, DiscountPerUnit = 0 }
            },
            PointTransactions = new List<PointTransaction>
            {
                new PointTransaction { CustomerId = customer3.CustomerId, PointChange = 90, TransactionType = "Earn", CreatedAt = DateTime.Now.AddHours(-5), Description = "Points earned from purchase" }
            }
        };

        dbContext.Invoices.AddRange(new[] { invoice1, invoice2, invoice3 });
        await dbContext.SaveChangesAsync();

        // Point Transaction for redemption
        var pointRedemption = new PointTransaction
        {
            CustomerId = customer2.CustomerId,
            PointChange = -300,
            TransactionType = "Redeem",
            CreatedAt = DateTime.Now.AddDays(-3),
            Description = "Redeemed for 10% Discount Coupon"
        };

        dbContext.PointTransactions.Add(pointRedemption);

        // Customer Rewards
        if (discountCoupon != null && shoppingBag != null)
        {
            var customerRewards = new[]
            {
                new CustomerReward { CustomerId = customer2.CustomerId, RewardId = discountCoupon.RewardId, RedeemedAt = DateTime.Now.AddDays(-3), Status = "Claimed" },
                new CustomerReward { CustomerId = customer1.CustomerId, RewardId = shoppingBag.RewardId, InvoiceId = invoice1.InvoiceId, RedeemedAt = DateTime.Now.AddDays(-2), Status = "Used", UsedAt = DateTime.Now.AddDays(-2) }
            };

            dbContext.CustomerRewards.AddRange(customerRewards);
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedOrderRequestsAsync(MegaMarketDbContext dbContext)
    {
        if (await dbContext.OrderRequests.AnyAsync())
        {
            return;
        }

        var products = await dbContext.Products.ToListAsync();
        if (!products.Any())
        {
            return;
        }

        var eggs = products.FirstOrDefault(p => p.Barcode == "3330003330003");
        var milk = products.FirstOrDefault(p => p.Barcode == "2220002220002");
        var apples = products.FirstOrDefault(p => p.Barcode == "1110001110001");

        if (eggs == null || milk == null || apples == null)
        {
            return;
        }

        var orderRequests = new[]
        {
            new OrderRequest { ProductId = eggs.ProductId, RequestedQuantity = 100, RequestDate = DateTime.Today.AddDays(-1), Status = "Pending" },
            new OrderRequest { ProductId = milk.ProductId, RequestedQuantity = 200, RequestDate = DateTime.Today.AddDays(-2), Status = "Ordered" },
            new OrderRequest { ProductId = apples.ProductId, RequestedQuantity = 150, RequestDate = DateTime.Today.AddDays(-5), Status = "Received" }
        };

        dbContext.OrderRequests.AddRange(orderRequests);
        await dbContext.SaveChangesAsync();
    }
}
