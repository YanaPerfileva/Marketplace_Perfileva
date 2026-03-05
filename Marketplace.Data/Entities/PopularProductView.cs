namespace Marketplace.Data.Entities
{
    public class PopularProductView
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string Category { get; set; } = string.Empty;
        public int OrdersCount { get; set; }
        public int SoldCount { get; set; }
        public decimal? AvgRating { get; set; }
        public int AvailableStock { get; set; }
    }
}
