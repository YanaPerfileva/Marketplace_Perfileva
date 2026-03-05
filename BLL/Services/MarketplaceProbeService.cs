using Marketplace.BLL.Interfaces;
using Marketplace.BLL.Models;
using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;
using Marketplace.Data.Context;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Marketplace.BLL.Services
{
    public sealed class MarketplaceProbeService : IMarketplaceProbeService
    {
        private readonly MarketplaceContext _context;
        private readonly IProductRepository _products;
        private readonly ICategoryRepository _categories;
        private readonly IPromotionRepository _promotions;
        private readonly IFavoriteRepository _favorites;
        private readonly IUserLogRepository _logs;
        private readonly ISellerRepository _sellers;

        public MarketplaceProbeService(
            MarketplaceContext context,
            IProductRepository products,
            ICategoryRepository categories,
            IPromotionRepository promotions,
            IFavoriteRepository favorites,
            IUserLogRepository logs,
            ISellerRepository sellers)
        {
            _context = context;
            _products = products;
            _categories = categories;
            _promotions = promotions;
            _favorites = favorites;
            _logs = logs;
            _sellers = sellers;
        }

        public Task<PaginatedResult<ProductDto>> GetProductsAsync(string? search, int page, int pageSize) =>
            _products.GetPaginatedProductsAsync(search: search, page: page, pageSize: pageSize);

        public Task<PaginatedResult<CategoryDto>> GetCategoriesAsync(int page, int pageSize) =>
            _categories.GetPaginatedCategoriesAsync(page: page, pageSize: pageSize);

        public Task<PaginatedResult<PromotionDto>> GetPromotionsAsync(int page, int pageSize) =>
            _promotions.GetPaginatedPromotionsAsync(page: page, pageSize: pageSize);

        public Task<PaginatedResult<FavoriteDto>> GetFavoritesAsync(int userId, int page, int pageSize) =>
            _favorites.GetPaginatedUserFavoritesAsync(userId, page, pageSize);

        public Task<PaginatedResult<UserLogDto>> GetUserLogsAsync(int? userId, int page, int pageSize) =>
            _logs.GetPaginatedUserLogsAsync(userId, page: page, pageSize: pageSize);

        public async Task<IReadOnlyCollection<PopularProductView>> GetPopularProductsViewAsync() =>
            await _context.PopularProducts.AsNoTracking().Take(100).ToListAsync();

        public async Task<IReadOnlyCollection<ActiveProductView>> GetActiveProductsViewAsync() =>
            await _context.ActiveProducts.AsNoTracking().Take(100).ToListAsync();

        public async Task<DashboardProbeResult> GetSellerDashboardProbeAsync(int sellerId)
        {
            var dashboard = await _sellers.GetSellerDashboardAsync(sellerId);
            var products = await _sellers.GetSellerProductsAsync(sellerId);
            var categories = await _sellers.GetSellerCategoriesAsync(sellerId);
            return new DashboardProbeResult
            {
                Dashboard = dashboard,
                Products = products,
                Categories = categories
            };
        }

        public async Task<OperationResult> VerifyPriceHistoryTriggerAsync(int sellerId, int categoryId)
        {
            var product = new Product
            {
                SellerId = sellerId,
                CategoryId = categoryId,
                Name = $"BLL-PriceTrigger-{Guid.NewGuid():N}"[..30],
                BasePrice = 100m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var sku = new ProductSku
            {
                ProductId = product.Id,
                SkuCode = $"BLL-PT-{Guid.NewGuid():N}"[..20],
                Price = 100m,
                Stock = 10,
                ReservedStock = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductSkus.Add(sku);
            await _context.SaveChangesAsync();

            sku.Price = 135m;
            await _context.SaveChangesAsync();

            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync();
            }

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM PriceHistory WHERE SkuId = @skuId AND OldPrice = @old AND NewPrice = @new";

            var skuParam = cmd.CreateParameter();
            skuParam.ParameterName = "@skuId";
            skuParam.Value = sku.Id;
            cmd.Parameters.Add(skuParam);

            var oldParam = cmd.CreateParameter();
            oldParam.ParameterName = "@old";
            oldParam.Value = 100m;
            cmd.Parameters.Add(oldParam);

            var newParam = cmd.CreateParameter();
            newParam.ParameterName = "@new";
            newParam.Value = 135m;
            cmd.Parameters.Add(newParam);

            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync() ?? 0);

            if (shouldClose)
            {
                await connection.CloseAsync();
            }

            return count > 0
                ? OperationResult.Ok($"Trigger worked: PriceHistory rows found = {count}.")
                : OperationResult.Fail("Trigger did not write row to PriceHistory.");
        }

        public async Task<OperationResult> VerifyOrderStockTriggerAsync(int userId, int sellerId, int categoryId)
        {
            var product = new Product
            {
                SellerId = sellerId,
                CategoryId = categoryId,
                Name = $"BLL-StockTrigger-{Guid.NewGuid():N}"[..30],
                BasePrice = 50m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var sku = new ProductSku
            {
                ProductId = product.Id,
                SkuCode = $"BLL-ST-{Guid.NewGuid():N}"[..20],
                Price = 50m,
                Stock = 10,
                ReservedStock = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductSkus.Add(sku);
            await _context.SaveChangesAsync();

            var order = new Order
            {
                UserId = userId,
                SellerId = sellerId,
                OrderNumber = $"BLL-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                TotalPrice = 0m,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.pending
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var sql = "INSERT INTO OrderItems (OrderId, SkuId, quantity, PriceAtTime, DiscountPercent) VALUES (@orderId, @skuId, @qty, @price, @disc)";

            var rejected = false;
            try
            {
                await _context.Database.ExecuteSqlRawAsync(sql,
                    new SqlParameter("@orderId", order.Id),
                    new SqlParameter("@skuId", sku.Id),
                    new SqlParameter("@qty", 20),
                    new SqlParameter("@price", sku.Price),
                    new SqlParameter("@disc", 0));
            }
            catch
            {
                rejected = true;
            }

            if (!rejected)
            {
                return OperationResult.Fail("Stock validation trigger did not reject oversized OrderItems insert.");
            }

            await _context.Database.ExecuteSqlRawAsync(sql,
                new SqlParameter("@orderId", order.Id),
                new SqlParameter("@skuId", sku.Id),
                new SqlParameter("@qty", 5),
                new SqlParameter("@price", sku.Price),
                new SqlParameter("@disc", 0));

            var refreshedSku = await _context.ProductSkus.AsNoTracking().FirstAsync(x => x.Id == sku.Id);

            return (refreshedSku.Stock == 5 && refreshedSku.ReservedStock == 5)
                ? OperationResult.Ok("Stock trigger worked: oversize insert rejected; valid insert updated stock/ReservedStock.")
                : OperationResult.Fail($"Unexpected stock values: stock={refreshedSku.Stock}, reserved={refreshedSku.ReservedStock}.");
        }

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
