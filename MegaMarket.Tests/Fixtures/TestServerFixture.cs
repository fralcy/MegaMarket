using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;
using MegaMarket.Tests.Helpers;

namespace MegaMarket.Tests.Fixtures;

public class TestServerFixture : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Replace real authentication with test authentication
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, options => { });

            // Remove the actual DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<MegaMarketDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Remove DbContextFactory if registered
            var factoryDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbContextFactory<MegaMarketDbContext>));
            if (factoryDescriptor != null)
            {
                services.Remove(factoryDescriptor);
            }

            // Add in-memory database - SAME NAME for both!
            services.AddDbContext<MegaMarketDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            services.AddDbContextFactory<MegaMarketDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });

            // Build the service provider and seed test data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MegaMarketDbContext>();

            // Ensure database is created
            db.Database.EnsureCreated();

            // Seed test data
            TestData.SeedTestData(db);
        });

        builder.UseEnvironment("Testing");
    }

    public async Task<T> ExecuteDbContextAsync<T>(Func<MegaMarketDbContext, Task<T>> action)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MegaMarketDbContext>();
        return await action(db);
    }

    public async Task ExecuteDbContextAsync(Func<MegaMarketDbContext, Task> action)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MegaMarketDbContext>();
        await action(db);
    }

    public T ExecuteDbContext<T>(Func<MegaMarketDbContext, T> action)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MegaMarketDbContext>();
        return action(db);
    }
}
