using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MegaMarket.Data.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MegaMarketDbContext>
    {
        public MegaMarketDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MegaMarketDbContext>();

            builder.UseSqlServer(
                "Server=localhost;Database=MegaMarket;uid=sa;pwd=Qwerty123456!;TrustServerCertificate=True");

            return new MegaMarketDbContext(builder.Options);
        }
    }
}
