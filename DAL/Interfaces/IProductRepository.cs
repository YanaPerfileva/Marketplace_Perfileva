using Marketplace.DAL.Models;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using System.Threading.Tasks;

namespace Marketplace.DAL.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetProductWithSellerAsync(int productId);
        Task<Product?> GetProductWithCategoriesAsync(int productId);
        Task<Product?> GetProductWithReviewsAsync(int productId);
        Task<Product?> GetProductWithImagesAsync(int productId);
        Task<Product?> GetProductWithSkusAsync(int productId);
        Task<PaginatedResult<ProductDto>> GetPaginatedProductsAsync(
            string? search = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false);
    }
}
