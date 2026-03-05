using Marketplace.DAL.Interfaces;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Marketplace.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MarketplaceContext context) : base(context)
        {
        }

        public async Task<User?> GetUserWithSellerAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Seller)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithOrdersAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithReviewsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithCartAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Carts)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithFavoritesAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Favorites)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithLogsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserLogs)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
