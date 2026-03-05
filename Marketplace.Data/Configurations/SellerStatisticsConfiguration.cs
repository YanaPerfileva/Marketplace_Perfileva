using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Marketplace.Data.Entities;

namespace Marketplace.Data.Configurations
{
    public class SellerStatisticsConfiguration : IEntityTypeConfiguration<SellerStatistics>
    {
        public void Configure(EntityTypeBuilder<SellerStatistics> builder)
        {
            builder.ToView("vw_SellerStatistics");
            builder.HasNoKey();


            builder.Property(s => s.AvgRating)
                .HasColumnType("decimal(5,2)");

            builder.Property(s => s.TotalRevenue)
                .HasColumnType("decimal(18,2)");
        }
    }
}
