using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Marketplace.Data.Entities;

namespace Marketplace.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.SellerId)
                .IsRequired();

            builder.Property(p => p.CategoryId)
                .IsRequired();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Brand)
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.BasePrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.MainImageUrl)
                .HasMaxLength(500);

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.ViewsCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.PurchaseCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.UpdatedAt)
                .HasColumnType("datetime2");

            builder.HasIndex(p => p.SellerId)
                .HasDatabaseName("IX_Products_SELLER");

            builder.HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Products_CATEGORY");

            builder.HasIndex(p => p.BasePrice)
                .HasDatabaseName("IX_Products_PRICE");

            builder.HasIndex(p => p.CreatedAt)
                .IsDescending()
                .HasDatabaseName("IX_Products_CREATED");

            builder.HasIndex(p => p.PurchaseCount)
                .IsDescending()
                .HasDatabaseName("IX_Products_POPULAR");

            builder.ToTable(t => t.HasCheckConstraint("CK_Products_PRICE", "BasePrice >= 0"));

            builder.HasOne(p => p.Seller)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SellerId);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            builder.HasMany(p => p.Skus)
                .WithOne(s => s.Product)
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId);

            builder.HasMany(p => p.Favorites)
                .WithOne(f => f.Product)
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.PromotionProducts)
                .WithOne(pp => pp.Product)
                .HasForeignKey(pp => pp.ProductId);
        }
    }
}
