using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;
using Marketplace.Data.Context;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Marketplace.DAL.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(MarketplaceContext context) : base(context)
        {
        }

        public async Task<Cart?> GetCartWithUserAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<Cart?> GetCartWithItemsAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<Cart?> GetCartWithItemsAndSkusAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductSku)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<Cart?> GetCartWithItemsAndProductsAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductSku)
                    .ThenInclude(sku => sku.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }
    }
}
