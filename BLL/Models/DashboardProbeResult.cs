using Marketplace.Data.Dto;

namespace Marketplace.BLL.Models
{
    public sealed class DashboardProbeResult
    {
        public SellerDashboardDto Dashboard { get; init; } = new();
        public IReadOnlyCollection<SellerProductDto> Products { get; init; } = Array.Empty<SellerProductDto>();
        public IReadOnlyCollection<CategoryDto> Categories { get; init; } = Array.Empty<CategoryDto>();
    }
}
