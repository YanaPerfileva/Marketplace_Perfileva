using Marketplace.Data.Entities;
using Marketplace.DAL.Models;
using System.Threading.Tasks;

namespace Marketplace.DAL.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartWithUserAsync(int cartId);
        Task<Cart?> GetCartWithItemsAsync(int cartId);
        Task<Cart?> GetCartWithItemsAndSkusAsync(int cartId);
        Task<Cart?> GetCartWithItemsAndProductsAsync(int cartId);
    }
}
