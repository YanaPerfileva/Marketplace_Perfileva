using Marketplace.Data.Entities;
using Marketplace.DAL.Models;
using System.Threading.Tasks;
using Marketplace.Data.Dto;

namespace Marketplace.DAL.Interfaces
{
    public interface IUserLogRepository : IGenericRepository<UserLog>
    {
        Task<UserLog?> GetUserLogWithUserAsync(int logId);
        Task<PaginatedResult<UserLogDto>> GetPaginatedUserLogsAsync(
            int? userId = null,
            string? entityType = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false);
    }
}
