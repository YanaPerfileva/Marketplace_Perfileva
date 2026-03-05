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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MarketplaceContext context) : base(context)
        {
        }

        public async Task<Category?> GetCategoryWithChildrenAsync(int categoryId)
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<Category?> GetCategoryWithProductsAsync(int categoryId)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<PaginatedResult<CategoryDto>> GetPaginatedCategoriesAsync(
            int? parentId = null,
            bool? isActive = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "SortOrder",
            bool ascending = false)
        {
            Expression<Func<Category, bool>> predicate = c =>
                (!parentId.HasValue || c.ParentId == parentId) &&
                (!isActive.HasValue || c.IsActive == isActive.Value);

            var entityPage = await GetPaginatedAsync(
                predicate,
                page,
                pageSize,
                sortBy,
                ascending
            );

            var dtoItems = entityPage.Items
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId,
                    ProductCount = c.Products != null ? c.Products.Count : 0
                })
                .ToList();

            return new PaginatedResult<CategoryDto>(dtoItems, entityPage.TotalCount, entityPage.Page, entityPage.PageSize);
        }
    }
}
