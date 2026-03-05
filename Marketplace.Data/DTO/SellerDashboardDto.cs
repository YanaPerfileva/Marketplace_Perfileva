using System;
using System.Collections.Generic;

namespace Marketplace.Data.Dto
{
    public class SellerDashboardDto
    {
        public int TotalProducts { get; set; }
        public int TotalSkus { get; set; }
        public int TotalStock { get; set; }
        public int TotalOrders { get; set; }
        public int ItemsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgRating { get; set; }

        public List<TopProductDto> TopProducts { get; set; } = new();

        public List<LowStockProductDto> LowStockProducts { get; set; } = new();
    }


}
