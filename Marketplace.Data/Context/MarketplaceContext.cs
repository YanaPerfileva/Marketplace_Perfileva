using System;
using System.Collections.Generic;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace Marketplace.Data.Context
{
    public class MarketplaceContext : DbContext
    {
        public MarketplaceContext(DbContextOptions<MarketplaceContext> options)
            : base(options)
        {
        }


        public DbSet<User> Users => Set<User>();

        public DbSet<Seller> Sellers => Set<Seller>();

        public DbSet<Category> Categories => Set<Category>();

        public DbSet<Product> Products => Set<Product>();

        public DbSet<ProductSku> ProductSkus => Set<ProductSku>();

        public DbSet<Cart> Carts => Set<Cart>();

        public DbSet<CartItem> CartItems => Set<CartItem>();

        public DbSet<Order> Orders => Set<Order>();

        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<Shipping> Shipments => Set<Shipping>();

        public DbSet<Review> Reviews => Set<Review>();

        public DbSet<Favorite> Favorites => Set<Favorite>();

        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();

        public DbSet<Promotion> Promotions => Set<Promotion>();

        public DbSet<PromotionProduct> PromotionProducts => Set<PromotionProduct>();

        public DbSet<UserLog> UserLogs => Set<UserLog>();


        public DbSet<PopularProductView> PopularProducts => Set<PopularProductView>();

        public DbSet<ActiveProductView> ActiveProducts => Set<ActiveProductView>();

        public DbSet<SellerStatistics> SellerStatistics => Set<SellerStatistics>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketplaceContext).Assembly);

            modelBuilder.Entity<SellerProductDto>().HasNoKey().ToView(null);
            modelBuilder.Entity<PopularProductView>().HasNoKey().ToView("v_popular_products");
            modelBuilder.Entity<ActiveProductView>().HasNoKey().ToView("v_active_products");
            modelBuilder.Entity<SellerStatistics>().HasNoKey().ToView(null);

        }


        public async Task<List<SellerProductDto>> GetSellerProductsAsync(int sellerId)
        {
            var sellerIdParam = new SqlParameter("@SellerId", sellerId);

            return await Set<SellerProductDto>()
                .FromSqlRaw("EXEC sp_GetSellerProducts @SellerId", sellerIdParam)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> AddProductAsync(
            int sellerId,
            int categoryId,
            string name,
            string? brand,
            string? description,
            decimal basePrice)
        {
            var newProductIdParam = new SqlParameter
            {
                ParameterName = "@NewProductId",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            var parameters = new[]
            {
                new SqlParameter("@SellerId", sellerId),
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@name", name),
                new SqlParameter("@brand", (object?)brand ?? DBNull.Value),
                new SqlParameter("@description", (object?)description ?? DBNull.Value),
                new SqlParameter("@BasePrice", basePrice),
                newProductIdParam
            };

            await Database.ExecuteSqlRawAsync(
                "EXEC sp_AddProduct @SellerId, @CategoryId, @name, @brand, @description, @BasePrice, @NewProductId OUTPUT",
                parameters);

            return (int)(newProductIdParam.Value ?? 0);
        }

        public async Task<int> AddProductSkuAsync(
            int productId,
            string? size,
            string? color,
            string? colorHex,
            decimal price,
            int stock)
        {
            var newSkuIdParam = new SqlParameter
            {
                ParameterName = "@NewSkuId",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            var parameters = new[]
            {
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@size", (object?)size ?? DBNull.Value),
                new SqlParameter("@color", (object?)color ?? DBNull.Value),
                new SqlParameter("@ColorHex", (object?)colorHex ?? DBNull.Value),
                new SqlParameter("@price", price),
                new SqlParameter("@stock", stock),
                newSkuIdParam
            };

            await Database.ExecuteSqlRawAsync(
                "EXEC sp_AddProductSKU @ProductId, @size, @color, @ColorHex, @price, @stock, @NewSkuId OUTPUT",
                parameters);

            return (int)(newSkuIdParam.Value ?? 0);
        }

        public async Task<SellerDashboardDto> GetSellerDashboardAsync(int sellerId)
        {
            var sellerIdParam = new SqlParameter("@SellerId", sellerId);

            using var connection = Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "sp_GetSellerDashboard";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(sellerIdParam);

            using var reader = await command.ExecuteReaderAsync();

            var dashboard = new SellerDashboardDto();
            if (await reader.ReadAsync())
            {
                var totalProductsOrdinal = reader.GetOrdinal("TotalProducts");
                var totalSkusOrdinal = reader.GetOrdinal("TotalSkus");
                var totalStockOrdinal = reader.GetOrdinal("TotalStock");
                var totalOrdersOrdinal = reader.GetOrdinal("TotalOrders");
                var itemsSoldOrdinal = reader.GetOrdinal("ItemsSold");
                var totalRevenueOrdinal = reader.GetOrdinal("TotalRevenue");
                var avgRatingOrdinal = reader.GetOrdinal("AvgRating");

                dashboard.TotalProducts = reader.IsDBNull(totalProductsOrdinal) ? 0 : reader.GetInt32(totalProductsOrdinal);
                dashboard.TotalSkus = reader.IsDBNull(totalSkusOrdinal) ? 0 : reader.GetInt32(totalSkusOrdinal);
                dashboard.TotalStock = reader.IsDBNull(totalStockOrdinal) ? 0 : reader.GetInt32(totalStockOrdinal);
                dashboard.TotalOrders = reader.IsDBNull(totalOrdersOrdinal) ? 0 : reader.GetInt32(totalOrdersOrdinal);
                dashboard.ItemsSold = reader.IsDBNull(itemsSoldOrdinal) ? 0 : reader.GetInt32(itemsSoldOrdinal);
                dashboard.TotalRevenue = reader.IsDBNull(totalRevenueOrdinal) ? 0 : reader.GetDecimal(totalRevenueOrdinal);
                dashboard.AvgRating = reader.IsDBNull(avgRatingOrdinal) ? 0 : reader.GetDecimal(avgRatingOrdinal);
            }

            if (await reader.NextResultAsync())
            {
                dashboard.TopProducts = new List<TopProductDto>();
                var nameOrdinal = reader.GetOrdinal("Name");
                var brandOrdinal = reader.GetOrdinal("Brand");
                var ordersCountOrdinal = reader.GetOrdinal("OrdersCount");
                var soldCountOrdinal = reader.GetOrdinal("SoldCount");
                var revenueOrdinal = reader.GetOrdinal("revenue");

                while (await reader.ReadAsync())
                {
                    dashboard.TopProducts.Add(new TopProductDto
                    {
                        Name = reader.IsDBNull(nameOrdinal) ? string.Empty : reader.GetString(nameOrdinal),
                        Brand = reader.IsDBNull(brandOrdinal) ? string.Empty : reader.GetString(brandOrdinal),
                        OrdersCount = reader.IsDBNull(ordersCountOrdinal) ? 0 : reader.GetInt32(ordersCountOrdinal),
                        SoldCount = reader.IsDBNull(soldCountOrdinal) ? 0 : reader.GetInt32(soldCountOrdinal),
                        Revenue = reader.IsDBNull(revenueOrdinal) ? 0 : reader.GetDecimal(revenueOrdinal)
                    });
                }
            }

            if (await reader.NextResultAsync())
            {
                dashboard.LowStockProducts = new List<LowStockProductDto>();
                var nameOrdinal = reader.GetOrdinal("Name");
                var sizeOrdinal = reader.GetOrdinal("Size");
                var colorOrdinal = reader.GetOrdinal("Color");
                var stockOrdinal = reader.GetOrdinal("Stock");

                while (await reader.ReadAsync())
                {
                    dashboard.LowStockProducts.Add(new LowStockProductDto
                    {
                        Name = reader.IsDBNull(nameOrdinal) ? string.Empty : reader.GetString(nameOrdinal),
                        Size = reader.IsDBNull(sizeOrdinal) ? string.Empty : reader.GetString(sizeOrdinal),
                        Color = reader.IsDBNull(colorOrdinal) ? string.Empty : reader.GetString(colorOrdinal),
                        Stock = reader.IsDBNull(stockOrdinal) ? 0 : reader.GetInt32(stockOrdinal)
                    });
                }
            }

            await connection.CloseAsync();
            return dashboard;
        }

        public async Task UpdateProductAsync(
            int productId,
            int sellerId,
            string? name,
            string? brand,
            string? description,
            decimal? basePrice,
            int? categoryId,
            bool? isActive)
        {
            var parameters = new List<SqlParameter>
            {
                new("@ProductId", productId),
                new("@SellerId", sellerId),
                new("@name", (object?)name ?? DBNull.Value),
                new("@brand", (object?)brand ?? DBNull.Value),
                new("@description", (object?)description ?? DBNull.Value),
                new("@BasePrice", (object?)basePrice ?? DBNull.Value),
                new("@CategoryId", (object?)categoryId ?? DBNull.Value),
                new("@IsActive", (object?)isActive ?? DBNull.Value)
            };

            await Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateProduct @ProductId, @SellerId, @name, @brand, @description, @BasePrice, @CategoryId, @IsActive",
                parameters.ToArray());
        }

        public async Task DeactivateProductAsync(int productId, int sellerId)
        {
            var parameters = new[]
            {
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@SellerId", sellerId)
            };

            await Database.ExecuteSqlRawAsync(
                "EXEC sp_DeactivateProduct @ProductId, @SellerId",
                parameters);
        }

        public async Task UpdateStockAsync(int skuId, int productId, int sellerId, int newStock)
        {
            var parameters = new[]
            {
                new SqlParameter("@SkuId", skuId),
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@SellerId", sellerId),
                new SqlParameter("@NewStock", newStock)
            };

            await Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateStock @SkuId, @ProductId, @SellerId, @NewStock",
                parameters);
        }

        public async Task BulkUpdateStockAsync(int sellerId, string updatesJson)
        {
            var parameters = new[]
            {
                new SqlParameter("@SellerId", sellerId),
                new SqlParameter("@updates", updatesJson)
            };

            await Database.ExecuteSqlRawAsync(
                "EXEC sp_BulkUpdateStock @SellerId, @updates",
                parameters);
        }

    }
}
