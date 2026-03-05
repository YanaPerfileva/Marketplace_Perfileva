namespace Marketplace.Data.Dto
{
    public class SellerDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}
