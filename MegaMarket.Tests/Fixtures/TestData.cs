namespace MegaMarket.Tests.Fixtures;

public static class TestData
{
    public static void SeedTestData(MegaMarketDbContext context)
    {
        // Clear existing data
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        SeedCustomers(context);
        SeedRewards(context);
        SeedProducts(context);
        SeedSuppliers(context);
        SeedUsers(context);
        SeedShiftTypes(context);
        SeedAttendances(context);

        context.SaveChanges();
    }

    private static void SeedCustomers(MegaMarketDbContext context)
    {
        var customers = new[]
        {
            new Customer
            {
                CustomerId = 1,
                FullName = "Nguyen Van A",
                Phone = "0901234567",
                Email = "nguyenvana@email.com",
                Points = 1000,
                Rank = "Gold"
            },
            new Customer
            {
                CustomerId = 2,
                FullName = "Tran Thi B",
                Phone = "0907654321",
                Email = "tranthib@email.com",
                Points = 500,
                Rank = "Silver"
            },
            new Customer
            {
                CustomerId = 3,
                FullName = "Le Van C",
                Phone = "0912345678",
                Email = "levanc@email.com",
                Points = 2000,
                Rank = "Platinum"
            }
        };

        context.Customers.AddRange(customers);
    }

    private static void SeedRewards(MegaMarketDbContext context)
    {
        var rewards = new[]
        {
            new Reward
            {
                RewardId = 1,
                Name = "10% Discount Voucher",
                Description = "Get 10% off your next purchase",
                PointCost = 100,
                RewardType = "Voucher",
                Value = 10,
                IsActive = true
            },
            new Reward
            {
                RewardId = 2,
                Name = "Free Shipping Voucher",
                Description = "Free shipping on orders over 500k",
                PointCost = 200,
                RewardType = "Voucher",
                Value = 0,
                IsActive = true
            },
            new Reward
            {
                RewardId = 3,
                Name = "50k Cash Voucher",
                Description = "50,000 VND discount voucher",
                PointCost = 500,
                RewardType = "Voucher",
                Value = 50000,
                IsActive = true
            }
        };

        context.Rewards.AddRange(rewards);
    }

    private static void SeedProducts(MegaMarketDbContext context)
    {
        var products = new[]
        {
            new Product
            {
                ProductId = 1,
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
                ProductId = 2,
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
                ProductId = 3,
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
                ProductId = 4,
                Barcode = "4440004440004",
                Name = "Roasted Coffee Beans 500g",
                Category = "Beverages",
                UnitLabel = "Bag",
                UnitPrice = 95000,
                QuantityInStock = 80,
                MinQuantity = 20,
                IsPerishable = false
            }
        };

        context.Products.AddRange(products);
    }

    private static void SeedSuppliers(MegaMarketDbContext context)
    {
        var suppliers = new[]
        {
            new Supplier
            {
                SupplierId = 1,
                Name = "Fresh Foods Co.",
                Phone = "555-1200"
            },
            new Supplier
            {
                SupplierId = 2,
                Name = "Dairy Direct",
                Phone = "555-8831"
            },
            new Supplier
            {
                SupplierId = 3,
                Name = "Global Groceries",
                Phone = "555-7412"
            }
        };

        context.Suppliers.AddRange(suppliers);
    }

    private static void SeedUsers(MegaMarketDbContext context)
    {
        var users = new[]
        {
            new User
            {
                UserId = 1,
                FullName = "Admin User",
                Username = "admin",
                Password = "Password123!",
                Role = "Admin"
            },
            new User
            {
                UserId = 2,
                FullName = "John Manager",
                Username = "john.manager",
                Password = "Password123!",
                Role = "Manager"
            }
        };

        context.Users.AddRange(users);
    }

    private static void SeedShiftTypes(MegaMarketDbContext context)
    {
        var shiftTypes = new[]
        {
            new ShiftType
            {
                ShiftTypeId = 1,
                Name = "Morning Shift",
                StartTime = new TimeSpan(6, 0, 0),   // 6:00 AM
                EndTime = new TimeSpan(14, 0, 0),     // 2:00 PM
                WagePerHour = 40000
            },
            new ShiftType
            {
                ShiftTypeId = 2,
                Name = "Afternoon Shift",
                StartTime = new TimeSpan(14, 0, 0),   // 2:00 PM
                EndTime = new TimeSpan(22, 0, 0),     // 10:00 PM
                WagePerHour = 45000
            },
            new ShiftType
            {
                ShiftTypeId = 3,
                Name = "Night Shift",
                StartTime = new TimeSpan(22, 0, 0),   // 10:00 PM
                EndTime = new TimeSpan(6, 0, 0),      // 6:00 AM
                WagePerHour = 50000
            }
        };

        context.ShiftTypes.AddRange(shiftTypes);
    }

    private static void SeedAttendances(MegaMarketDbContext context)
    {
        var attendances = new[]
        {
            new MegaMarket.Data.Models.Attendance
            {
                AttendanceId = 1,
                UserId = 1,
                ShiftTypeId = 1,
                Date = DateTime.Today,
                CheckIn = DateTime.Today.AddHours(6).AddMinutes(5),
                CheckOut = DateTime.Today.AddHours(14).AddMinutes(10),
                IsLate = true,
                Note = "Test attendance for admin user"
            },
            new MegaMarket.Data.Models.Attendance
            {
                AttendanceId = 2,
                UserId = 2,
                ShiftTypeId = 2,
                Date = DateTime.Today,
                CheckIn = DateTime.Today.AddHours(14),
                CheckOut = DateTime.Today.AddHours(22),
                IsLate = false,
                Note = "Test attendance for manager user"
            },
            new MegaMarket.Data.Models.Attendance
            {
                AttendanceId = 3,
                UserId = 1,
                ShiftTypeId = 1,
                Date = DateTime.Today.AddDays(-1),
                CheckIn = DateTime.Today.AddDays(-1).AddHours(6),
                CheckOut = DateTime.Today.AddDays(-1).AddHours(14),
                IsLate = false,
                Note = "Yesterday's attendance"
            }
        };

        context.Attendances.AddRange(attendances);
    }

    // Factory methods for creating test entities
    public static Customer CreateCustomer(
        string fullName = "Test Customer",
        string phone = "0900000000",
        string? email = null,
        int points = 0)
    {
        return new Customer
        {
            FullName = fullName,
            Phone = phone,
            Email = email ?? $"{phone}@test.com",
            Points = points,
            Rank = "Silver"
        };
    }

    public static Reward CreateReward(
        string name = "Test Reward",
        int pointCost = 100,
        string rewardType = "Voucher",
        decimal value = 10)
    {
        return new Reward
        {
            Name = name,
            Description = $"Test description for {name}",
            PointCost = pointCost,
            RewardType = rewardType,
            Value = value,
            IsActive = true
        };
    }

    public static Product CreateProduct(
        string barcode = "9999999999999",
        string name = "Test Product",
        decimal unitPrice = 10000,
        int quantityInStock = 100)
    {
        return new Product
        {
            Barcode = barcode,
            Name = name,
            Category = "Test",
            UnitLabel = "Unit",
            UnitPrice = unitPrice,
            QuantityInStock = quantityInStock,
            MinQuantity = 10,
            IsPerishable = false
        };
    }
}
