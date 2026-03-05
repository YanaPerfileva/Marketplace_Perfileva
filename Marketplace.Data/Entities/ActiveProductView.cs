namespace Marketplace.Data.Entities
{
    public class ActiveProductView
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int TotalStock { get; set; }
    }
}
