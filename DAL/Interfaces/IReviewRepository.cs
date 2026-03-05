using Marketplace.DAL.Models;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using System.Threading.Tasks;

namespace Marketplace.DAL.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<Review?> GetReviewWithUserAsync(int reviewId);
        Task<Review?> GetReviewWithProductAsync(int reviewId);
        Task<Review?> GetReviewWithOrderAsync(int reviewId);
        Task<Review?> GetReviewWithUserAndProductAsync(int reviewId);
        Task<PaginatedResult<ReviewDto>> GetPaginatedProductReviewsAsync(
            int productId,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false);
    }
}
