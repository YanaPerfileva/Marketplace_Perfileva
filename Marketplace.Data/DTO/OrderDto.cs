using System;
using System.Collections.Generic;

namespace Marketplace.Data.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public int ItemsCount { get; set; }

        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

        public class OrderItemDto
    {
        public int Id { get; set; }
        public int SkuId { get; set; }
        public string SkuCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal PriceAtTime { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}
