using Marketplace.Data.Entities;
using Marketplace.DAL.Models;
using System.Threading.Tasks;
using Marketplace.Data.Dto;

namespace Marketplace.DAL.Interfaces
{
    public interface IPromotionRepository : IGenericRepository<Promotion>
    {
        Task<Promotion?> GetPromotionWithProductsAsync(int promotionId);
        Task<Promotion?> GetPromotionWithProductsAndCategoriesAsync(int promotionId);
        Task<PaginatedResult<PromotionDto>> GetPaginatedPromotionsAsync(
            bool? isActive = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "StartDate",
            bool ascending = false);
    }
}
