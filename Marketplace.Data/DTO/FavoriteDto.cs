using System;

namespace Marketplace.Data.Dto
{
    public class FavoriteDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;
        public string ProductBrand { get; set; } = string.Empty;
        public string ProductCategoryName { get; set; } = string.Empty;
        public decimal ProductBasePrice { get; set; }
        public string ProductMainImageUrl { get; set; } = string.Empty;
        public int ProductTotalStock { get; set; }
        public decimal ProductMinPrice { get; set; }
        public decimal ProductMaxPrice { get; set; }
    }
}
