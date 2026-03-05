using System;

namespace Marketplace.Data.Dto
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
