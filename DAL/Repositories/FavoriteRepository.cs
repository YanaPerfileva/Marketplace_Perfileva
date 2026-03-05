using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Marketplace.Data.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.DAL.Repositories
{
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(MarketplaceContext context) : base(context)
        {
        }

        public async Task<Favorite?> GetFavoriteWithProductAsync(int favoriteId)
        {
            return await _context.Favorites
                .Include(f => f.Product)
                .FirstOrDefaultAsync(f => f.Id == favoriteId);
        }

        public async Task<Favorite?> GetFavoriteWithUserAsync(int favoriteId)
        {
            return await _context.Favorites
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == favoriteId);
        }

        public async Task<PaginatedResult<FavoriteDto>> GetPaginatedUserFavoritesAsync(
            int userId,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            IQueryable<Favorite> query = _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.User)
                .Include(f => f.Product).ThenInclude(p => p.Category)
                .Include(f => f.Product).ThenInclude(p => p.ProductImages)
                .Include(f => f.Product).ThenInclude(p => p.Skus);

            var sortProperty = sortBy;
            if (string.Equals(sortBy, "CreatedAt", StringComparison.OrdinalIgnoreCase))
            {
                sortProperty = "CreatedAt";
            }
            else if (string.Equals(sortBy, "ProductName", StringComparison.OrdinalIgnoreCase))
            {
                sortProperty = "Product.Name";
            }
            else if (string.Equals(sortBy, "ProductPrice", StringComparison.OrdinalIgnoreCase))
            {
                sortProperty = "Product.BasePrice";
            }

            var totalCount = await query.CountAsync();

            if (!string.IsNullOrWhiteSpace(sortProperty))
            {
                query = ApplySorting(query, sortProperty, ascending);
            }

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FavoriteDto
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    ProductId = f.ProductId,
                    CreatedAt = f.CreatedAt,
                    UserEmail = f.User != null ? f.User.Email : string.Empty,
                    UserFullName = f.User != null ? f.User.FullName : string.Empty,
                    ProductName = f.Product != null ? f.Product.Name : string.Empty,
                    ProductBrand = f.Product != null ? (f.Product.Brand ?? string.Empty) : string.Empty,
                    ProductCategoryName = f.Product != null && f.Product.Category != null ? f.Product.Category.Name : string.Empty,
                    ProductBasePrice = f.Product != null ? f.Product.BasePrice : 0m,
                    ProductMainImageUrl = f.Product != null
                        ? f.Product.ProductImages.OrderBy(pi => pi.Id).Select(pi => pi.ImageUrl).FirstOrDefault() ?? string.Empty
                        : string.Empty,
                    ProductTotalStock = f.Product != null ? f.Product.Skus.Sum(s => s.Stock) : 0,
                    ProductMinPrice = f.Product != null && f.Product.Skus.Any()
                        ? f.Product.Skus.Min(s => s.Price)
                        : (f.Product != null ? f.Product.BasePrice : 0m),
                    ProductMaxPrice = f.Product != null && f.Product.Skus.Any()
                        ? f.Product.Skus.Max(s => s.Price)
                        : (f.Product != null ? f.Product.BasePrice : 0m)
                })
                .ToListAsync();

            return new PaginatedResult<FavoriteDto>(items, totalCount, page, pageSize);
        }
    }
}
