using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MegaMarket.Data.Data;
using MegaMarket.API.Services;
using MegaMarket.API.GraphQL;
using MegaMarket.API.GraphQL.Types;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register DbContext
builder.Services.AddDbContext<MegaMarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ShiftTypeService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<AuthService>();

// Dashboard Services
builder.Services.AddScoped<DashboardSalesService>();
builder.Services.AddScoped<DashboardInventoryService>();

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

// CORS (nếu frontend và backend khác origin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
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

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();