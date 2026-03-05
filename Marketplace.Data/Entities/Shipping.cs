using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Marketplace.Data.Enums;

namespace Marketplace.Data.Entities
{
    [Table("Shipping")]
    public class Shipping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [MaxLength(150)]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string RecipientPhone { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = "Россия";

        [Required]
        [MaxLength(50)]
        [EnumDataType(typeof(ShippingMethod))]
        public ShippingMethod ShippingMethod { get; set; }

        [Required]
        [MaxLength(50)]
        public ShippingStatus Status { get; set; } = ShippingStatus.pending;

        [MaxLength(100)]
        public string? TrackingNumber { get; set; }

        public DateTime? EstimatedDelivery { get; set; }

        public DateTime? DeliveredAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}
