using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.Data.Entities
{
    [NotMapped]
    public class SellerStatistics
    {
        public int SellerId { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int ItemsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgRating { get; set; }
    }
}
