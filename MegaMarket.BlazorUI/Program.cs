using MegaMarket.BlazorUI.Components;
using MegaMarket.BlazorUI.Services.Auth;
using MegaMarket.BlazorUI.Services.GraphQL;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add LocalStorage service
builder.Services.AddScoped<LocalStorageService>();

// Add Auth services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();

// Configure HttpClient for GraphQL
builder.Services.AddHttpClient("GraphQL", client =>
{
    var graphqlEndpoint = builder.Configuration["GraphQL:Endpoint"] ?? "https://localhost:7284/graphql";
    client.BaseAddress = new Uri(graphqlEndpoint);
});

// Add GraphQL client service
builder.Services.AddScoped<GraphQLClient>();

// Add ApexCharts service
builder.Services.AddApexCharts();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
