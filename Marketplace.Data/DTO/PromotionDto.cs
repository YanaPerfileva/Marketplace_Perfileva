using System;
using System.Collections.Generic;

namespace Marketplace.Data.Dto
{
    public class PromotionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ProductCount { get; set; }
        public List<PromotionProductDto> Products { get; set; } = new List<PromotionProductDto>();
    }

    public class PromotionProductDto
    {
        public int PromotionId { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string ProductBrand { get; set; } = string.Empty;
        public string ProductCategoryName { get; set; } = string.Empty;
        public decimal ProductBasePrice { get; set; }
        public string ProductMainImageUrl { get; set; } = string.Empty;
    }
}
