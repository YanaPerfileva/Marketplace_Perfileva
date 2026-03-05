using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.Data.Entities
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SellerId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Brand { get; set; }

        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }

        [MaxLength(500)]
        public string? MainImageUrl { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public int ViewsCount { get; set; } = 0;

        [Required]
        public int PurchaseCount { get; set; } = 0;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }


        [ForeignKey("SellerId")]
        public virtual Seller Seller { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<ProductSku> Skus { get; set; } = new HashSet<ProductSku>();

        public virtual ICollection<Cart> Carts { get; set; } = new HashSet<Cart>();

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

        public virtual ICollection<Review> Reviews { get; set; } = new HashSet<Review>();

        public virtual ICollection<Favorite> Favorites { get; set; } = new HashSet<Favorite>();

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new HashSet<ProductImage>();

        public virtual ICollection<PromotionProduct> PromotionProducts { get; set; } = new HashSet<PromotionProduct>();
    }
}
