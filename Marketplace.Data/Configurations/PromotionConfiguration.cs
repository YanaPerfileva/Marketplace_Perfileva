using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;

namespace Marketplace.Data.Configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotions");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.DiscountType)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion(
                    v => v.ToString(),
                    v => (DiscountType)Enum.Parse(typeof(DiscountType), v));

            builder.Property(p => p.DiscountValue)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.StartDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(p => p.EndDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.HasIndex(p => new { p.StartDate, p.EndDate })
                .HasDatabaseName("IX_Promotions_DATES");

            builder.ToTable(t => t.HasCheckConstraint("CK_Promotions_DATE", "StartDate < EndDate"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Promotions_TYPE",
                "DiscountType IN ('percent', 'fixed')"));

            builder.HasMany(p => p.PromotionProducts)
                .WithOne(pp => pp.Promotion)
                .HasForeignKey(pp => pp.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
