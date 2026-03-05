namespace Marketplace.Data.Dto
{
    public class LowStockProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int Stock { get; set; }
    }
}
