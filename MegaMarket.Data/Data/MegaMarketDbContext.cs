using Microsoft.EntityFrameworkCore;
using MegaMarket.Data.Models;

namespace MegaMarket.Data.Data;

public class MegaMarketDbContext : DbContext
{
    public MegaMarketDbContext(DbContextOptions<MegaMarketDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<ShiftType> ShiftTypes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Import> Imports { get; set; }
    public DbSet<ImportDetail> ImportDetails { get; set; }
    public DbSet<OrderRequest> OrderRequests { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<PromotionProduct> PromotionProducts { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<CustomerReward> CustomerRewards { get; set; }
    public DbSet<PointTransaction> PointTransactions { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure composite keys for junction tables
        modelBuilder.Entity<ImportDetail>()
            .HasKey(id => new { id.ImportId, id.ProductId });

        modelBuilder.Entity<InvoiceDetail>()
            .HasKey(id => new { id.InvoiceId, id.ProductId });

        modelBuilder.Entity<PromotionProduct>()
            .HasKey(pp => new { pp.PromotionId, pp.ProductId });

        // User relationships
        modelBuilder.Entity<User>()
            .HasMany(u => u.Imports)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Invoices)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Attendances)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Customer relationships
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Invoices)
            .WithOne(i => i.Customer)
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.CustomerRewards)
            .WithOne(cr => cr.Customer)
            .HasForeignKey(cr => cr.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .HasMany(c => c.PointTransactions)
            .WithOne(pt => pt.Customer)
            .HasForeignKey(pt => pt.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Supplier relationships
        modelBuilder.Entity<Supplier>()
            .HasMany(s => s.Imports)
            .WithOne(i => i.Supplier)
            .HasForeignKey(i => i.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // ShiftType relationships
        modelBuilder.Entity<ShiftType>()
            .HasMany(st => st.Attendances)
            .WithOne(a => a.ShiftType)
            .HasForeignKey(a => a.ShiftTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product relationships
        modelBuilder.Entity<Product>()
            .HasMany(p => p.ImportDetails)
            .WithOne(id => id.Product)
            .HasForeignKey(id => id.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.InvoiceDetails)
            .WithOne(id => id.Product)
            .HasForeignKey(id => id.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderRequests)
            .WithOne(or => or.Product)
            .HasForeignKey(or => or.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.PromotionProducts)
            .WithOne(pp => pp.Product)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Import relationships
        modelBuilder.Entity<Import>()
            .HasMany(i => i.ImportDetails)
            .WithOne(id => id.Import)
            .HasForeignKey(id => id.ImportId)
            .OnDelete(DeleteBehavior.Cascade);

        // Invoice relationships
        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.InvoiceDetails)
            .WithOne(id => id.Invoice)
            .HasForeignKey(id => id.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.PointTransactions)
            .WithOne(pt => pt.Invoice)
            .HasForeignKey(pt => pt.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Promotion)
            .WithMany(p => p.Invoices)
            .HasForeignKey(i => i.PromotionId)
            .OnDelete(DeleteBehavior.SetNull);

        // Promotion relationships
        modelBuilder.Entity<Promotion>()
            .HasMany(p => p.InvoiceDetails)
            .WithOne(id => id.Promotion)
            .HasForeignKey(id => id.PromotionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Promotion>()
            .HasMany(p => p.PromotionProducts)
            .WithOne(pp => pp.Promotion)
            .HasForeignKey(pp => pp.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Reward relationships
        modelBuilder.Entity<Reward>()
            .HasMany(r => r.CustomerRewards)
            .WithOne(cr => cr.Reward)
            .HasForeignKey(cr => cr.RewardId)
            .OnDelete(DeleteBehavior.Restrict);

        // CustomerReward relationships
        modelBuilder.Entity<CustomerReward>()
            .HasOne(cr => cr.Invoice)
            .WithMany()
            .HasForeignKey(cr => cr.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes for better performance
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Barcode)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Phone);

        modelBuilder.Entity<Invoice>()
            .HasIndex(i => i.CreatedAt);

        modelBuilder.Entity<Import>()
            .HasIndex(i => i.ImportDate);

        modelBuilder.Entity<Attendance>()
            .HasIndex(a => new { a.UserId, a.Date });

        // Dashboard-specific indexes for better query performance
        modelBuilder.Entity<Invoice>()
            .HasIndex(i => i.Status);

        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.QuantityInStock, p.MinQuantity });

        modelBuilder.Entity<Product>()
            .HasIndex(p => new { p.IsPerishable, p.ExpiryDate });

        modelBuilder.Entity<Product>()
            .HasIndex(p => p.Category);

        modelBuilder.Entity<PointTransaction>()
            .HasIndex(pt => new { pt.CustomerId, pt.CreatedAt, pt.TransactionType });
    }
}
