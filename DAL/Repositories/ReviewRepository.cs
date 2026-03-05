using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;
using Marketplace.Data.Context;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Marketplace.DAL.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(MarketplaceContext context) : base(context)
        {
        }

        public async Task<Review?> GetReviewWithUserAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<Review?> GetReviewWithProductAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<Review?> GetReviewWithOrderAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.Order)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<Review?> GetReviewWithUserAndProductAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<PaginatedResult<ReviewDto>> GetPaginatedProductReviewsAsync(
            int productId,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false)
        {
            Expression<Func<Review, bool>> predicate = r => r.ProductId == productId;

            var entityPage = await GetPaginatedAsync(
                predicate,
                page,
                pageSize,
                sortBy,
                ascending
            );

            var dtoItems = entityPage.Items
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Rating = (decimal)r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            return new PaginatedResult<ReviewDto>(dtoItems, entityPage.TotalCount, page, pageSize);
        }
    }
}
