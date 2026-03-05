using Marketplace.DAL.Interfaces;
using Marketplace.DAL.Models;
using Marketplace.Data.Context;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Marketplace.DAL.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(MarketplaceContext context) : base(context)
        {
        }

        public async Task<Order?> GetOrderWithUserAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderWithSellerAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Seller)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderWithItemsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductSku)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderWithPaymentAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderWithShippingAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Shipping)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetOrderWithReviewsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Reviews)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<PaginatedResult<OrderDto>> GetPaginatedUserOrdersAsync(
            int userId,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false)
        {
            Expression<Func<Order, bool>> predicate = o => o.UserId == userId;

            var entityPage = await GetPaginatedAsync(
                predicate,
                page,
                pageSize,
                sortBy,
                ascending
            );

            var dtoItems = entityPage.Items
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber ?? string.Empty,
                    Status = o.Status.ToString(),
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt,
                    ItemsCount = o.OrderItems != null ? o.OrderItems.Count : 0,
                    Items = new List<OrderItemDto>()
                })
                .ToList();

            return new PaginatedResult<OrderDto>(dtoItems, entityPage.TotalCount, entityPage.Page, entityPage.PageSize);
        }
    }
}
