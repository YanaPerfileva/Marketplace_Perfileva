using Marketplace.DAL.Models;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using System.Threading.Tasks;

namespace Marketplace.DAL.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithUserAsync(int orderId);
        Task<Order?> GetOrderWithSellerAsync(int orderId);
        Task<Order?> GetOrderWithItemsAsync(int orderId);
        Task<Order?> GetOrderWithPaymentAsync(int orderId);
        Task<Order?> GetOrderWithShippingAsync(int orderId);
        Task<Order?> GetOrderWithReviewsAsync(int orderId);
        Task<PaginatedResult<OrderDto>> GetPaginatedUserOrdersAsync(
            int userId,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false);
    }
}
