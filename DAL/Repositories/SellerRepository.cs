using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;
using Marketplace.Data.Context;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.DAL.Repositories
{
    public class SellerRepository : GenericRepository<Marketplace.Data.Entities.Seller>, ISellerRepository
    {
        public SellerRepository(MarketplaceContext context) : base(context)
        {
        }

        public async Task<Marketplace.Data.Entities.Seller?> GetSellerWithUserAsync(int sellerId)
        {
            return await _context.Sellers
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == sellerId);
        }

        public async Task<Marketplace.Data.Entities.Seller?> GetSellerWithProductsAsync(int sellerId)
        {
            return await _context.Sellers
                .Include(s => s.Products)
                    .ThenInclude(p => p.Skus)
                .Include(s => s.Products)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == sellerId);
        }

        public async Task<Marketplace.Data.Entities.Seller?> GetSellerWithDetailedInfoAsync(int sellerId)
        {
            return await _context.Sellers
                .Include(s => s.User)
                .Include(s => s.Products)
                    .ThenInclude(p => p.Skus)
                .Include(s => s.Products)
                    .ThenInclude(p => p.ProductImages)
                .Include(s => s.Products)
                    .ThenInclude(p => p.Reviews)
                .FirstOrDefaultAsync(s => s.Id == sellerId);
        }

        public async Task<PaginatedResult<SellerProductDto>> GetPaginatedSellerProductsAsync(
     int sellerId, int page = 1, int pageSize = 10, string sortBy = "CreatedAt", bool ascending = false)
        {
            // ВРЕМЕННО - возвращаем пустой результат (пока нет процедуры)
            return new PaginatedResult<SellerProductDto>(new List<SellerProductDto>(), 0, page, pageSize);
        }

        public async Task<List<SellerProductDto>> GetSellerProductsAsync(int sellerId)
        {
            var param = new SqlParameter("@SellerId", sellerId);
            return await _context.Set<SellerProductDto>()
                .FromSqlRaw("EXEC sp_GetSellerProducts @SellerId", param)
                .AsNoTracking()
                .ToListAsync();
        }

        // ИСПРАВЛЕНО: Убрано ручное управление соединением
        public async Task<SellerDashboardDto> GetSellerDashboardAsync(int sellerId)
        {
            var param = new SqlParameter("@SellerId", sellerId);

            // Создаем временный view model для чтения результата
            var dashboard = await _context.Database
                .SqlQueryRaw<SellerDashboardDto>("EXEC sp_GetSellerDashboard @SellerId", param)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return dashboard ?? new SellerDashboardDto();
        }

        public async Task<int> AddProductAsync(int sellerId, int categoryId, string name, string brand,
            string description, decimal basePrice)
        {
            var newProductIdParam = new SqlParameter("@NewProductId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            var parameters = new[]
            {
                new SqlParameter("@SellerId", sellerId),
                new SqlParameter("@CategoryId", categoryId),
                new SqlParameter("@name", name),
                new SqlParameter("@brand", brand),
                new SqlParameter("@description", description),
                new SqlParameter("@BasePrice", basePrice),
                newProductIdParam
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddProduct @SellerId, @CategoryId, @name, @brand, @description, @BasePrice, @NewProductId OUTPUT",
                parameters);

            return (int)newProductIdParam.Value;
        }

        public async Task<int> AddProductSkuAsync(int productId, string? size, string? color,
            string? colorHex, decimal price, int stock)
        {
            var newSkuIdParam = new SqlParameter("@NewSkuId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            var parameters = new[]
            {
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@size", (object)size ?? DBNull.Value),
                new SqlParameter("@color", (object)color ?? DBNull.Value),
                new SqlParameter("@ColorHex", (object)colorHex ?? DBNull.Value),
                new SqlParameter("@price", price),
                new SqlParameter("@stock", stock),
                newSkuIdParam
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddProductSKU @ProductId, @size, @color, @ColorHex, @price, @stock, @NewSkuId OUTPUT",
                parameters);

            return (int)newSkuIdParam.Value;
        }

        public async Task UpdateProductAsync(int productId, int sellerId, string? name, string? brand,
            string? description, decimal? basePrice, int? categoryId, bool? isActive)
        {
            var parameters = new[]
            {
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@SellerId", sellerId),
                new SqlParameter("@name", (object)name ?? DBNull.Value),
                new SqlParameter("@brand", (object)brand ?? DBNull.Value),
                new SqlParameter("@description", (object)description ?? DBNull.Value),
                new SqlParameter("@BasePrice", (object)basePrice ?? DBNull.Value),
                new SqlParameter("@CategoryId", (object)categoryId ?? DBNull.Value),
                new SqlParameter("@IsActive", (object)isActive ?? DBNull.Value)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateProduct @ProductId, @SellerId, @name, @brand, @description, @BasePrice, @CategoryId, @IsActive",
                parameters);
        }

        public async Task DeactivateProductAsync(int productId, int sellerId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_DeactivateProduct @ProductId, @SellerId",
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@SellerId", sellerId));
        }

        public async Task ActivateProductAsync(int productId, int sellerId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_ActivateProduct @ProductId, @SellerId",
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@SellerId", sellerId));
        }

        public async Task UpdateStockAsync(int skuId, int productId, int sellerId, int newStock)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateStock @SkuId, @ProductId, @SellerId, @NewStock",
                new SqlParameter("@SkuId", skuId),
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@SellerId", sellerId),
                new SqlParameter("@NewStock", newStock));
        }

        public async Task BulkUpdateStockAsync(int sellerId, string updatesJson)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_BulkUpdateStock @SellerId, @updates",
                new SqlParameter("@SellerId", sellerId),
                new SqlParameter("@updates", updatesJson));
        }

        public async Task<List<CategoryDto>> GetSellerCategoriesAsync(int sellerId)
        {
            var param = new SqlParameter("@SellerId", sellerId);
            return await _context.Set<CategoryDto>()
                .FromSqlRaw("EXEC sp_GetSellerCategories @SellerId", param)
                .AsNoTracking()
                .ToListAsync();
        }
        //Добавила
        // СОЗДАНИЕ ТОВАРА + SKU + ГЛАВНОЕ ФОТО
        public async Task<int> AddProductWithSkuAsync(int sellerId, int categoryId, string name, string brand,
            string description, decimal basePrice, string? mainImageUrl = null)
        {
            var product = new Product
            {
                SellerId = sellerId,
                CategoryId = categoryId,
                Name = name,
                Brand = brand,
                Description = description,
                BasePrice = basePrice,
                MainImageUrl = mainImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // АВТОМАТИЧЕСКИ создаем 1 SKU (базовый вариант)
            var sku = new ProductSku
            {
                ProductId = product.Id,
                SkuCode = $"SKU-{product.Id}-{DateTime.UtcNow:HHmmss}",
                Price = basePrice,
                Stock = 0,  // Продавец сам пополнит
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductSkus.Add(sku);
            await _context.SaveChangesAsync();

            return product.Id;
        }

        // ИЗМЕНЕНИЕ ТОВАРА
        public async Task<bool> UpdateProductAsync(int productId, int sellerId, string name,
            string? brand, string? description, decimal basePrice, int categoryId, string? mainImageUrl)
        {
            var product = await _context.Products
                .Include(p => p.Skus) // ✅ Загружаем SKU!
                .FirstOrDefaultAsync(p => p.Id == productId && p.SellerId == sellerId);

            if (product == null)
                return false; // ✅ Возвращаем false

            product.Name = name;
            product.Brand = brand;
            product.Description = description;
            product.BasePrice = basePrice;
            product.CategoryId = categoryId;  // ✅ Обязательное!
            product.MainImageUrl = mainImageUrl;
            product.UpdatedAt = DateTime.UtcNow;
            product.IsActive = true;          // ✅ Безопасность

            await _context.SaveChangesAsync();
            return true; // ✅ Успех!
        }

        // ДОБАВИТЬ ФОТО к товару
        public async Task<int> AddProductImageAsync(int productId, string imageUrl, int? skuId = null, bool isMain = false)
        {
            // Если главное - сбрасываем у других
            if (isMain)
            {
                await _context.ProductImages
                    .Where(img => img.ProductId == productId && img.IsMain)
                    .ExecuteUpdateAsync(s => s.SetProperty(img => img.IsMain, false));
            }

            var image = new ProductImage
            {
                ProductId = productId,
                SkuId = skuId,
                ImageUrl = imageUrl,  // ТОЛЬКО ImageUrl!
                IsMain = isMain,
                SortOrder = await _context.ProductImages
                    .Where(img => img.ProductId == productId)
                    .CountAsync(),
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductImages.Add(image);
            await _context.SaveChangesAsync();
            return image.Id;
        }
    }
}
