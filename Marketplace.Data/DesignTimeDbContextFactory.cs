using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Marketplace.Data.Context;
using System;

namespace Marketplace.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MarketplaceContext>
    {
        public MarketplaceContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MarketplaceContext>();

            optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;",
            x => x.CommandTimeout(120));
            return new MarketplaceContext(optionsBuilder.Options);
        }
    }
}
