using System;
using Marketplace.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Wpf.Infrastructure
{
    public static class DbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder UseMarketplaceSqlServer(
            this DbContextOptionsBuilder optionsBuilder,
            string connectionString)
        {
            return optionsBuilder.UseSqlServer(connectionString, sql =>
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
        }

        public static DbContextOptionsBuilder<MarketplaceContext> UseMarketplaceSqlServer(
            this DbContextOptionsBuilder<MarketplaceContext> optionsBuilder,
            string connectionString)
        {
            return optionsBuilder.UseSqlServer(connectionString, sql =>
                sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
        }
    }
}
