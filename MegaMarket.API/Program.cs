using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MegaMarket.Data.Data;
using MegaMarket.API.Services;
using MegaMarket.API.GraphQL;
using MegaMarket.API.GraphQL.Types;
using MegaMarket.API.Services.Interfaces;
using MegaMarket.API.Services.Implementations;
using MegaMarket.API.Data;
using MegaMarket.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register DbContext
builder.Services.AddDbContext<MegaMarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Configure GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<UserType>()
    .AddType<ShiftTypeType>()
    .AddType<AttendanceType>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

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
app.MapGraphQL("/graphql");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MegaMarketDbContext>();
    await DbSeeder.SeedAsync(dbContext);
}

app.Run();