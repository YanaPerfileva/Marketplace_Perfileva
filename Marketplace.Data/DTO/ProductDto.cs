using System;
using System.Collections.Generic;

namespace Marketplace.Data.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int ViewsCount { get; set; }
        public int PurchaseCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }

        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int TotalStock { get; set; }
        public int SkuCount { get; set; }
        public int OrdersCount { get; set; }
        public int SoldCount { get; set; }
        public decimal Revenue { get; set; }

        public string? Size { get; set; }
        public string? Color { get; set; }
        public int? Stock { get; set; }

        public List<SkuDto> Skus { get; set; } = new List<SkuDto>();
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
    }

    public class SkuDto
    {
        public int Id { get; set; }
        public string SkuCode { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? ColorHex { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int ReservedStock { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProductImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
