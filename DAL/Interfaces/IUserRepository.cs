using Marketplace.Data.Entities;
using Marketplace.DAL.Models;
using System.Threading.Tasks;

namespace Marketplace.DAL.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUserWithSellerAsync(int userId);
        Task<User?> GetUserWithOrdersAsync(int userId);
        Task<User?> GetUserWithReviewsAsync(int userId);
        Task<User?> GetUserWithCartAsync(int userId);
        Task<User?> GetUserWithFavoritesAsync(int userId);
        Task<User?> GetUserWithLogsAsync(int userId);
    }
}
