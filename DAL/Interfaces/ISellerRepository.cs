using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.Data.Dto;
using Marketplace.DAL.Models;

namespace Marketplace.DAL.Interfaces
{
    public interface ISellerRepository : IGenericRepository<Marketplace.Data.Entities.Seller>
    {
        Task<Marketplace.Data.Entities.Seller?> GetSellerWithUserAsync(int sellerId);
        Task<Marketplace.Data.Entities.Seller?> GetSellerWithProductsAsync(int sellerId);
        Task<Marketplace.Data.Entities.Seller?> GetSellerWithDetailedInfoAsync(int sellerId);

        Task<PaginatedResult<SellerProductDto>> GetPaginatedSellerProductsAsync(
            int sellerId,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false);

        Task<List<SellerProductDto>> GetSellerProductsAsync(int sellerId);
        Task<SellerDashboardDto> GetSellerDashboardAsync(int sellerId);

        Task<int> AddProductAsync(
            int sellerId,
            int categoryId,
            string name,
            string brand,
            string description,
            decimal basePrice);

        Task<int> AddProductSkuAsync(
            int productId,
            string? size,
            string? color,
            string? colorHex,
            decimal price,
            int stock);

        Task UpdateProductAsync(
            int productId,
            int sellerId,
            string? name,
            string? brand,
            string? description,
            decimal? basePrice,
            int? categoryId,
            bool? isActive);

        Task DeactivateProductAsync(int productId, int sellerId);
        Task ActivateProductAsync(int productId, int sellerId);

        Task UpdateStockAsync(int skuId, int productId, int sellerId, int newStock);
        Task BulkUpdateStockAsync(int sellerId, string updatesJson);

        Task<List<CategoryDto>> GetSellerCategoriesAsync(int sellerId);
    }
}
