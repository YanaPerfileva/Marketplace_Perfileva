namespace Marketplace.Data.Dto
{
    public class SellerStatisticsDto
    {
        public int SellerId { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
