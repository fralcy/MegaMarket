using Microsoft.EntityFrameworkCore;
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

// Swagger (giữ lại nếu còn dùng REST API)
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

app.UseAuthorization();

app.MapControllers();

// Map GraphQL endpoint
app.MapGraphQL("/graphql");

app.Run();