using System;

namespace Marketplace.Data.Dto
{
    public class TopProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public int OrdersCount { get; set; }
        public int SoldCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
