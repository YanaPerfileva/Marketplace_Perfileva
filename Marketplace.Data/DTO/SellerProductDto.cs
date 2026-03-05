using System;

namespace Marketplace.Data.Dto
{
    public class SellerProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }
        public int ViewsCount { get; set; }
        public int PurchaseCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int TotalStock { get; set; }
        public int SkuCount { get; set; }
    }
}
