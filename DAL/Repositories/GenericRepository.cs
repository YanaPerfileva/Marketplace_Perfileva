using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Marketplace.Data.Context;
using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;

namespace Marketplace.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly MarketplaceContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(MarketplaceContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = await _dbSet.SingleOrDefaultAsync(predicate);
            return entity;
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<PaginatedResult<T>> GetPaginatedAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize,
            string? sortBy = null,
            bool ascending = true)
        {
            return await GetPaginatedInternalAsync(
                _dbSet.Where(predicate),
                page,
                pageSize,
                q => string.IsNullOrWhiteSpace(sortBy) ? q : ApplySorting(q, sortBy!, ascending));
        }

        public virtual async Task<PaginatedResult<T>> GetPaginatedAsync(
            Expression<Func<T, bool>> predicate,
            int page,
            int pageSize,
            IDictionary<string, bool> sortByFields)
        {
            return await GetPaginatedInternalAsync(
                _dbSet.Where(predicate),
                page,
                pageSize,
                q => ApplyMultiSorting(q, sortByFields));
        }

        public virtual async Task<PaginatedResult<T>> GetPaginatedAsync(
            IQueryable<T> query,
            int page,
            int pageSize,
            string? sortBy = null,
            bool ascending = true)
        {
            return await GetPaginatedInternalAsync(
                query,
                page,
                pageSize,
                q => string.IsNullOrWhiteSpace(sortBy) ? q : ApplySorting(q, sortBy!, ascending));
        }

        public virtual async Task<PaginatedResult<T>> GetPaginatedAsync(
            IQueryable<T> query,
            int page,
            int pageSize,
            IDictionary<string, bool> sortByFields)
        {
            return await GetPaginatedInternalAsync(
                query,
                page,
                pageSize,
                q => ApplyMultiSorting(q, sortByFields));
        }

        private async Task<PaginatedResult<T>> GetPaginatedInternalAsync(
            IQueryable<T> query,
            int page,
            int pageSize,
            Func<IQueryable<T>, IQueryable<T>> sorting)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = await query.CountAsync();
            query = sorting(query);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>(items, totalCount, page, pageSize);
        }

        private IQueryable<T> ApplyMultiSorting(IQueryable<T> query, IDictionary<string, bool> sortByFields)
        {
            if (sortByFields == null || sortByFields.Count == 0)
            {
                return query;
            }

            var isFirst = true;
            foreach (var field in sortByFields)
            {
                query = ApplySorting(query, field.Key, field.Value, isFirst);
                isFirst = false;
            }

            return query;
        }

        public virtual async Task<IEnumerable<T>> FindSortedAsync(
            Expression<Func<T, bool>> predicate,
            string? sortBy = null,
            bool ascending = true,
            int? skip = null,
            int? take = null)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = ApplySorting(query, sortBy!, ascending);
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindSortedAsync(
            Expression<Func<T, bool>> predicate,
            IDictionary<string, bool> sortByFields,
            int? skip = null,
            int? take = null)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            if (sortByFields != null && sortByFields.Count > 0)
            {
                bool isFirst = true;
                foreach (var field in sortByFields)
                {
                    query = ApplySorting(query, field.Key, field.Value, isFirst);
                    isFirst = false;
                }
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        protected virtual IQueryable<T> ApplySorting(IQueryable<T> query, string sortBy, bool ascending, bool isFirst = true)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? propertyAccess = null;
            foreach (var member in sortBy.Split('.'))
            {
                propertyAccess = propertyAccess == null
                    ? Expression.PropertyOrField(parameter, member)
                    : Expression.PropertyOrField(propertyAccess, member);
            }

            if (propertyAccess == null)
                return query;

            var propertyType = propertyAccess.Type;
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), propertyType);
            var lambda = Expression.Lambda(delegateType, propertyAccess, parameter);

            string methodName;
            if (isFirst)
                methodName = ascending ? "OrderBy" : "OrderByDescending";
            else
                methodName = ascending ? "ThenBy" : "ThenByDescending";

            var queryableMethods = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static);
            var method = queryableMethods
                .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                .Single()
                .MakeGenericMethod(typeof(T), propertyType);

            var result = method.Invoke(null, new object[] { query, lambda });
            return (IQueryable<T>)result!;
        }
    }
}
