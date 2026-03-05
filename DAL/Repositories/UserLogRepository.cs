using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Marketplace.Data.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Marketplace.DAL.Repositories
{
    public class UserLogRepository : GenericRepository<UserLog>, IUserLogRepository
    {
        public UserLogRepository(MarketplaceContext context) : base(context)
        {
        }

        public async Task<UserLog?> GetUserLogWithUserAsync(int logId)
        {
            return await _context.UserLogs
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == logId);
        }

        public async Task<PaginatedResult<UserLogDto>> GetPaginatedUserLogsAsync(
            int? userId = null,
            string? entityType = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false)
        {
            Expression<Func<UserLog, bool>> predicate = l =>
                (!userId.HasValue || l.UserId == userId) &&
                (string.IsNullOrEmpty(entityType) || l.EntityType == entityType);

            var normalizedSortBy = sortBy?.Trim().ToLowerInvariant() switch
            {
                "CreatedAt" => nameof(UserLog.CreatedAt),
                "UserId" => nameof(UserLog.UserId),
                "EntityType" => nameof(UserLog.EntityType),
                "action" => nameof(UserLog.Action),
                _ => nameof(UserLog.CreatedAt)
            };

            var query = _context.UserLogs
                .Where(predicate)
                .AsQueryable();

            query = ApplySorting(query, normalizedSortBy, ascending);

            var projectedQuery = query
                .Select(l => new UserLogDto
                {
                    Id = l.Id,
                    UserId = l.UserId ?? 0,
                    Action = l.Action,
                    EntityType = l.EntityType ?? string.Empty,
                    EntityId = l.EntityId ?? 0,
                    Details = l.Details ?? string.Empty,
                    IpAddress = l.IpAddress ?? string.Empty,
                    CreatedAt = l.CreatedAt,
                });

            var totalCount = await projectedQuery.CountAsync();
            var items = await projectedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<UserLogDto>(items, totalCount, page, pageSize);
        }
    }
}
