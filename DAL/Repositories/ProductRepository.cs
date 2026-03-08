using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;
using Marketplace.Data.Context;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Marketplace.DAL.Repositories
{

    //public class ProductRepository : GenericRepository<Product>, IProductRepository
    //{
    //    public ProductRepository(MarketplaceContext context) : base(context)
    //    {
    //    }

    //    public async Task<Product?> GetProductWithSellerAsync(int productId)
    //    {
    //        return await _context.Products
    //            .Include(p => p.Seller)
    //            .FirstOrDefaultAsync(p => p.Id == productId);
    //    }

    //    public async Task<Product?> GetProductWithCategoriesAsync(int productId)
    //    {
    //        return await _context.Products
    //            .Include(p => p.Category)
    //            .FirstOrDefaultAsync(p => p.Id == productId);
    //    }

    //    public async Task<Product?> GetProductWithReviewsAsync(int productId)
    //    {
    //        return await _context.Products
    //            .Include(p => p.Reviews)
    //            .FirstOrDefaultAsync(p => p.Id == productId);
    //    }

    //    public async Task<Product?> GetProductWithImagesAsync(int productId)
    //    {
    //        return await _context.Products
    //            .Include(p => p.ProductImages)
    //            .FirstOrDefaultAsync(p => p.Id == productId);
    //    }

    //    public async Task<Product?> GetProductWithSkusAsync(int productId)
    //    {
    //        return await _context.Products
    //            .Include(p => p.Skus)
    //            .FirstOrDefaultAsync(p => p.Id == productId);
    //    }

    //    public async Task<PaginatedResult<ProductDto>> GetPaginatedProductsAsync(
    //        string? search = null,
    //        int? categoryId = null,
    //        decimal? minPrice = null,
    //        decimal? maxPrice = null,
    //        int page = 1,
    //        int pageSize = 10,
    //        string sortBy = "CreatedAt",
    //        bool ascending = false)
    //    {
    //        Expression<Func<Product, bool>> predicate = p =>
    //            (string.IsNullOrEmpty(search) || (p.Name != null && p.Name.Contains(search!)) || (p.Brand != null && p.Brand.Contains(search!))) &&
    //            (!categoryId.HasValue || p.CategoryId == categoryId) &&
    //            (!minPrice.HasValue || p.BasePrice >= minPrice.Value) &&
    //            (!maxPrice.HasValue || p.BasePrice <= maxPrice.Value);

    //        var entityPage = await GetPaginatedAsync(
    //            predicate,
    //            page,
    //            pageSize,
    //            sortBy,
    //            ascending
    //        );

    //        var dtoItems = entityPage.Items
    //            .Select(p => new ProductDto
    //            {
    //                Id = p.Id,
    //                Name = p.Name ?? string.Empty,
    //                Brand = p.Brand ?? string.Empty,
    //                Description = p.Description ?? string.Empty,
    //                BasePrice = p.BasePrice,
    //                MainImageUrl = p.MainImageUrl ?? (p.ProductImages != null && p.ProductImages.Any()
    //                    ? p.ProductImages.OrderBy(pi => pi.Id).Select(pi => pi.ImageUrl).FirstOrDefault() ?? string.Empty
    //                    : string.Empty),
    //                IsActive = p.IsActive,
    //                ViewsCount = p.ViewsCount,
    //                PurchaseCount = p.PurchaseCount,
    //                CreatedAt = p.CreatedAt,
    //                UpdatedAt = p.UpdatedAt,
    //                CategoryName = p.Category != null ? p.Category.Name ?? string.Empty : string.Empty,
    //                SellerName = p.Seller != null && p.Seller.User != null ? p.Seller.User.FullName ?? string.Empty : string.Empty,
    //                MinPrice = (p.Skus != null && p.Skus.Any()) ? p.Skus.Min(s => s.Price) : p.BasePrice,
    //                MaxPrice = (p.Skus != null && p.Skus.Any()) ? p.Skus.Max(s => s.Price) : p.BasePrice,
    //                TotalStock = (p.Skus != null) ? p.Skus.Sum(s => s.Stock) : 0,
    //                SkuCount = (p.Skus != null) ? p.Skus.Count : 0
    //            })
    //            .ToList();

    //        return new PaginatedResult<ProductDto>(dtoItems, entityPage.TotalCount, entityPage.Page, entityPage.PageSize);
    //    }
    //}
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(MarketplaceContext context) : base(context)
        {
        }
    

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()//Евгений
        {
            return await _context.Categories
                                 .OrderBy(c => c.Name)
                                 .ToListAsync();
        }

        public async Task<Product?> GetProductWithSellerAsync(int productId)//Евгений
        {
            return await _context.Products
              .Include(p => p.Seller)
              .Include(p => p.Skus)
              .Where(p => p.IsActive)
              .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<List<Product>> GetAllProductsAsync()//Евгений
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Skus)
                .ToListAsync();
        }


        public async Task<Product?> GetProductWithCategoriesAsync(int productId)
        {
            return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Skus)
            .FirstOrDefaultAsync(p => p.Id == productId);
        }


        public async Task<Product?> GetProductWithReviewsAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<Product?> GetProductWithImagesAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<Product?> GetProductWithSkusAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Skus)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<PaginatedResult<ProductDto>> GetPaginatedProductsAsync(
            string? search = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false)
        {
            Expression<Func<Product, bool>> predicate = p =>
                (string.IsNullOrEmpty(search) || (p.Name != null && p.Name.Contains(search!)) || (p.Brand != null && p.Brand.Contains(search!))) &&
                (!categoryId.HasValue || p.CategoryId == categoryId) &&
                (!minPrice.HasValue || p.BasePrice >= minPrice.Value) &&
                (!maxPrice.HasValue || p.BasePrice <= maxPrice.Value);

            if (sortBy == "Price") sortBy = "BasePrice"; //Евгений

            var entityPage = await GetPaginatedAsync(
                predicate,
                page,
                pageSize,
                sortBy,
                ascending
            );

            var dtoItems = entityPage.Items
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name ?? string.Empty,
                    Brand = p.Brand ?? string.Empty,
                    Description = p.Description ?? string.Empty,
                    BasePrice = p.BasePrice,
                    MainImageUrl = p.MainImageUrl ?? (p.ProductImages != null && p.ProductImages.Any()
                        ? p.ProductImages.OrderBy(pi => pi.Id).Select(pi => pi.ImageUrl).FirstOrDefault() ?? string.Empty
                        : string.Empty),
                    IsActive = p.IsActive,
                    ViewsCount = p.ViewsCount,
                    PurchaseCount = p.PurchaseCount,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CategoryName = p.Category != null ? p.Category.Name ?? string.Empty : string.Empty,
                    SellerName = p.Seller != null && p.Seller.User != null ? p.Seller.User.FullName ?? string.Empty : string.Empty,
                    MinPrice = (p.Skus != null && p.Skus.Any()) ? p.Skus.Min(s => s.Price) : p.BasePrice,
                    MaxPrice = (p.Skus != null && p.Skus.Any()) ? p.Skus.Max(s => s.Price) : p.BasePrice,
                    TotalStock = (p.Skus != null) ? p.Skus.Sum(s => s.Stock) : 0,
                    SkuCount = (p.Skus != null) ? p.Skus.Count : 0
                })
                .ToList();

            return new PaginatedResult<ProductDto>(dtoItems, entityPage.TotalCount, entityPage.Page, entityPage.PageSize);
        }
    }

}
