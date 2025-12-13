using MegaMarket.BlazorUI.Components;
using MegaMarket.Data.Data;
using MegaMarket.Data.DataAccess;
using MegaMarket.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using MegaMarket.BlazorUI.Services.Auth;
using MegaMarket.BlazorUI.Services.GraphQL;
using Microsoft.AspNetCore.Components.Authorization;
using MegaMarket.BlazorUI.Services;
using MegaMarket.BlazorUI.Services.Products;
using MegaMarket.BlazorUI.Services.Imports;
using MegaMarket.BlazorUI.Services.CustomerLoyalty;
using MegaMarket.BlazorUI.Services.Dashboard;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient("MegaMarket.API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7284/");
});
/*builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<InvoiceDAO>();
builder.Services.AddDbContextFactory<MegaMarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    x => x.MigrationsAssembly("MegaMarket.Data")));*/

// Add LocalStorage service
builder.Services.AddScoped<LocalStorageService>();

// Add Auth services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
}).AddCookie("Cookies", options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
    // Don't redirect or challenge - let Blazor components handle authorization
    options.Events.OnRedirectToLogin = context => Task.CompletedTask;
    options.Events.OnRedirectToAccessDenied = context => Task.CompletedTask;
});
builder.Services.AddAuthorizationCore();

// Configure HttpClient for GraphQL
builder.Services.AddHttpClient("GraphQL", client =>
{
    var graphqlEndpoint = builder.Configuration["GraphQL:Endpoint"] ?? "https://localhost:7284/graphql";
    client.BaseAddress = new Uri(graphqlEndpoint);
});

// Add GraphQL client service
builder.Services.AddScoped<GraphQLClient>();

// Configure API Base URL
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7284/";

// Add Dashboard API client service
builder.Services.AddHttpClient<DashboardApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Register Product & Import Services with HttpClient
builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<ProductService>((sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ApiSettings:BaseUrl"] ?? configuration["ApiBaseUrl"] ?? "https://localhost:7284";
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHttpClient<ImportService>((sp, client) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ApiSettings:BaseUrl"] ?? configuration["ApiBaseUrl"] ?? "https://localhost:7284";
    client.BaseAddress = new Uri(baseUrl);
});

// Register HTTP Client
builder.Services.AddHttpClient();

// Register Customer Loyalty Services
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<LoyaltyService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<RewardManagementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();