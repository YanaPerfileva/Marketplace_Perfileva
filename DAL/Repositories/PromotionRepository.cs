using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Marketplace.Data.Dto;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Marketplace.DAL.Repositories
{
    public class PromotionRepository : GenericRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(MarketplaceContext context) : base(context)
        {
        }

        public async Task<Promotion?> GetPromotionWithProductsAsync(int promotionId)
        {
            return await _context.Promotions
                .Include(p => p.PromotionProducts)
                .ThenInclude(pp => pp.Product)
                .FirstOrDefaultAsync(p => p.Id == promotionId);
        }

        public async Task<Promotion?> GetPromotionWithProductsAndCategoriesAsync(int promotionId)
        {
            return await _context.Promotions
                .Include(p => p.PromotionProducts)
                    .ThenInclude(pp => pp.Product)
                    .ThenInclude(prod => prod.Category)
                .FirstOrDefaultAsync(p => p.Id == promotionId);
        }

        public async Task<PaginatedResult<PromotionDto>> GetPaginatedPromotionsAsync(
            bool? isActive = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "StartDate",
            bool ascending = false)
        {
            var query = _context.Promotions.AsQueryable();

            if (isActive.HasValue)
                query = query.Where(p => p.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();

            var sortedQuery = ascending
                ? query.OrderBy(p => EF.Property<object>(p, sortBy))
                : query.OrderByDescending(p => EF.Property<object>(p, sortBy));

            var items = await sortedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PromotionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    DiscountType = p.DiscountType.ToString(),
                    DiscountValue = p.DiscountValue,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return new PaginatedResult<PromotionDto>(items, totalCount, page, pageSize);
        }
    }
}
