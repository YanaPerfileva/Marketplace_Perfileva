using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Marketplace.DAL.Models;

namespace Marketplace.DAL.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        Task<PaginatedResult<T>> GetPaginatedAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize,
            string? sortBy = null,
            bool ascending = true);

        Task<PaginatedResult<T>> GetPaginatedAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize,
            IDictionary<string, bool> sortByFields);

        Task<IEnumerable<T>> FindSortedAsync(
            Expression<Func<T, bool>> predicate,
            string? sortBy = null,
            bool ascending = true,
            int? skip = null,
            int? take = null);

        Task<IEnumerable<T>> FindSortedAsync(
            Expression<Func<T, bool>> predicate,
            IDictionary<string, bool> sortByFields,
            int? skip = null,
            int? take = null);
    }
}
