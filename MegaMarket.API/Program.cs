using System.Text;
using System.Text.Json.Serialization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MegaMarket.Data.Data;
using MegaMarket.Data.DataAccess;
using MegaMarket.Data.Repositories;
using MegaMarket.API.Services;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.API.Services.Implementations;
using MegaMarket.API.Data;
using MegaMarket.Data.Repositories.Interfaces;
using MegaMarket.Data.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references in JSON serialization
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<InvoiceDAO>();

// Register DbContext
builder.Services.AddDbContextFactory<MegaMarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    x => x.MigrationsAssembly("MegaMarket.Data")));

// Product & Import Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IImportService, ImportService>();

// Customer & Reward Services
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IPointTransactionRepository, PointTransactionRepository>();
builder.Services.AddScoped<ICustomerRewardRepository, CustomerRewardRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IPointTransactionService, PointTransactionService>();
builder.Services.AddScoped<IRewardRepository, RewardRepository>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<ICustomerRewardService, CustomerRewardService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Employee Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ShiftTypeService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<AuthService>();

// Dashboard Services
builder.Services.AddScoped<DashboardSalesService>();
builder.Services.AddScoped<DashboardInventoryService>();
builder.Services.AddScoped<DashboardCustomerService>();
builder.Services.AddScoped<DashboardEmployeeService>();

// Promotion Services
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<PromotionDAO>();

// CORS (chỉ giữ 1 config)
var allowedOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>() ??
                     new[] { "https://localhost:7168", "http://localhost:5023", "https://localhost:5023" };
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new Exception("JWT Key chưa được config trong appsettings.json!");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MegaMarket";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MegaMarket";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Register DbContext for dependency injection (in addition to factory)
builder.Services.AddScoped<MegaMarketDbContext>(provider =>
{
    var factory = provider.GetRequiredService<IDbContextFactory<MegaMarketDbContext>>();
    return factory.CreateDbContext();
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// DEBUG: Endpoint to check current user claims
app.MapGet("/api/debug/claims", (HttpContext context) =>
{
    var user = context.User;
    var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();
    return Results.Ok(new
    {
        IsAuthenticated = user.Identity?.IsAuthenticated ?? false,
        Claims = claims,
        UserId = user.FindFirst("userId")?.Value,
        Role = user.FindFirst("role")?.Value,
        NameIdentifier = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
        ClaimsRole = user.FindFirst(ClaimTypes.Role)?.Value
    });
}).RequireAuthorization();

// TEMPORARY: Endpoint to hash existing passwords
app.MapPost("/api/admin/hash-passwords", async (MegaMarketDbContext dbContext) =>
{
    var users = await dbContext.Users.ToListAsync();
    foreach (var user in users)
    {
        // Only hash if password is plain text (not already hashed)
        if (!user.Password.StartsWith("$2"))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        }
    }
    await dbContext.SaveChangesAsync();
    return Results.Ok(new { message = "Passwords hashed successfully", count = users.Count });
});

// Seed database only if empty (non-testing environments)
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MegaMarketDbContext>();

        // Only seed if database is completely empty
        if (!await dbContext.Users.AnyAsync())
        {
            Console.WriteLine("Database is empty. Seeding initial data...");
            await DbSeeder.SeedAsync(dbContext);
            Console.WriteLine("Database seeding completed successfully!");
        }
        else
        {
            Console.WriteLine("Database already contains data. Skipping seed.");
        }
    }
}

app.Run();

// Make the implicit Program class public for integration tests
public partial class Program { }