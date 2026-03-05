using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Marketplace.Data.Entities;

namespace Marketplace.Data.Configurations
{
    public class SellerConfiguration : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.ToTable("Sellers");
            builder.HasKey(s => s.Id);


            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();


            builder.Property(s => s.UserId)
                .IsRequired();

            builder.Property(s => s.StoreName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(500);

            builder.Property(s => s.LogoUrl)
                .HasMaxLength(500);

            builder.Property(s => s.Rating)
                .HasColumnType("decimal(3,2)");

            builder.Property(s => s.TotalSales)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(s => s.Verified)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.HasIndex(s => s.StoreName)
                .IsUnique()
                .HasDatabaseName("UQ_SELLER_STORE");

            builder.HasIndex(s => s.Rating)
                .IsDescending()
                .HasDatabaseName("IX_Sellers_RATING");

            builder.HasIndex(s => s.Verified)
                .HasDatabaseName("IX_Sellers_VERIFIED");

            builder.HasOne(s => s.User)
                .WithOne(u => u.Seller)
                .HasForeignKey<Seller>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Products)
                .WithOne(p => p.Seller)
                .HasForeignKey(p => p.SellerId);

            builder.HasMany(s => s.Orders)
                .WithOne(o => o.Seller)
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
