using Marketplace.DAL.Models;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using System.Threading.Tasks;

namespace Marketplace.DAL.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category?> GetCategoryWithChildrenAsync(int categoryId);
        Task<Category?> GetCategoryWithProductsAsync(int categoryId);
        Task<PaginatedResult<CategoryDto>> GetPaginatedCategoriesAsync(
            int? parentId = null,
            bool? isActive = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "SortOrder",
            bool ascending = false);
    }
}
