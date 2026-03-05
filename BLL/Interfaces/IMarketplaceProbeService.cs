using Marketplace.BLL.Models;
using Marketplace.DAL.Models;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;

namespace Marketplace.BLL.Interfaces
{
    public interface IMarketplaceProbeService
    {
        Task<PaginatedResult<ProductDto>> GetProductsAsync(string? search, int page, int pageSize);
        Task<PaginatedResult<CategoryDto>> GetCategoriesAsync(int page, int pageSize);
        Task<PaginatedResult<PromotionDto>> GetPromotionsAsync(int page, int pageSize);
        Task<PaginatedResult<FavoriteDto>> GetFavoritesAsync(int userId, int page, int pageSize);
        Task<PaginatedResult<UserLogDto>> GetUserLogsAsync(int? userId, int page, int pageSize);

        Task<IReadOnlyCollection<PopularProductView>> GetPopularProductsViewAsync();
        Task<IReadOnlyCollection<ActiveProductView>> GetActiveProductsViewAsync();

        Task<DashboardProbeResult> GetSellerDashboardProbeAsync(int sellerId);
        Task<OperationResult> VerifyPriceHistoryTriggerAsync(int sellerId, int categoryId);
        Task<OperationResult> VerifyOrderStockTriggerAsync(int userId, int sellerId, int categoryId);
        Task<int> SaveChangesAsync();
    }
}
