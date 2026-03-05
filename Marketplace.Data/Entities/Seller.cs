using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.Data.Entities
{
    [Table("Sellers")]
    public class Seller
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string StoreName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        [Range(0, 5)]
        public decimal? Rating { get; set; }

        [Required]
        public int TotalSales { get; set; } = 0;

        [Required]
        public bool Verified { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
