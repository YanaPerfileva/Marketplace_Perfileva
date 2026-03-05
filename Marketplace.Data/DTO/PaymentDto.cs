using System;

namespace Marketplace.Data.Dto
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Method { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
