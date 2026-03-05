using System;

namespace Marketplace.Data.Dto
{
    public class ShippingDto
    {
        public int Id { get; set; }
        public string? Address { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
