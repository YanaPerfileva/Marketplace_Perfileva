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

        //public async Task<User?> GetUserWithOrdersAsync(int userId)
        //{
        //    return await _context.Users
        //        .Include(u => u.Orders)
        //        .FirstOrDefaultAsync(u => u.Id == userId);
        //}

        //public async Task<User?> GetUserWithReviewsAsync(int userId)
        //{
        //    return await _context.Users
        //        .Include(u => u.Reviews)
        //        .FirstOrDefaultAsync(u => u.Id == userId);
        //}

        //public async Task<User?> GetUserWithCartAsync(int userId)
        //{
        //    return await _context.Users
        //        .Include(u => u.Carts)
        //        .FirstOrDefaultAsync(u => u.Id == userId);
        //}

        //public async Task<User?> GetUserWithFavoritesAsync(int userId)
        //{
        //    return await _context.Users
        //        .Include(u => u.Favorites)
        //        .FirstOrDefaultAsync(u => u.Id == userId);
        //}

        public async Task<User?> GetUserWithLogsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserLogs)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<IEnumerable<Seller>> GetAllSellersAsync()//Евгений
        {


            return await _context.Sellers
                .OrderBy(s => s.StoreName)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithOrdersAsync(int userId)//Евгений
        {
            return await _context.Users
            .Include(u => u.Orders)
            .ThenInclude(o => o.OrderItems)
            .ThenInclude(i => i.ProductSku)
            .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithReviewsAsync(int userId)//Евгений
        {
            return await _context.Users
            .Include(u => u.Reviews)
            .ThenInclude(r => r.Product)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithCartAsync(int userId)//Евгений
        {
            return await _context.Users
           .Include(u => u.Carts.Where(c => c.IsActive))
           .ThenInclude(c => c.CartItems)
           .ThenInclude(ci => ci.ProductSku)
           .ThenInclude(s => s.Product)
           .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithFavoritesAsync(int userId)//Евгений
        {
            return await _context.Users
            .Include(u => u.Favorites)
            .ThenInclude(f => f.Product)
            .FirstOrDefaultAsync(u => u.Id == userId);
        }

    }
}
