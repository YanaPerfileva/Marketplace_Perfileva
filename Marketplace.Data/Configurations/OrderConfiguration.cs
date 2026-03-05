using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;

namespace Marketplace.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .ValueGeneratedOnAdd();

            builder.Property(o => o.UserId)
                .IsRequired();

            builder.Property(o => o.SellerId)
                .IsRequired();

            builder.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion(
                    v => v.ToString(),
                    v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v))
                .HasDefaultValue(OrderStatus.pending);

            builder.Property(o => o.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.DiscountAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(o => o.FinalPrice)
                .HasColumnType("decimal(18,2)")
                .HasComputedColumnSql("[TotalPrice] - [DiscountAmount]", stored: true);

            builder.Property(o => o.Comment)
                .HasMaxLength(500);

            builder.Property(o => o.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(o => o.UpdatedAt)
                .HasColumnType("datetime2");

            builder.HasIndex(o => o.OrderNumber)
                .IsUnique()
                .HasDatabaseName("UQ_ORDER_NUMBER");

            builder.HasIndex(o => o.UserId)
                .HasDatabaseName("IX_Orders_USER");

            builder.HasIndex(o => o.SellerId)
                .HasDatabaseName("IX_Orders_SELLER");

            builder.HasIndex(o => o.Status)
                .HasDatabaseName("IX_Orders_STATUS");

            builder.HasIndex(o => o.CreatedAt)
                .IsDescending()
                .HasDatabaseName("IX_Orders_CREATED");

            builder.HasIndex(o => new { o.UserId, o.Status })
                .HasDatabaseName("IX_Orders_USER_STATUS");

            builder.ToTable(t => t.HasCheckConstraint("CK_Orders_PRICE", "TotalPrice >= 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Orders_DISCOUNT", "DiscountAmount >= 0"));
            builder.ToTable(t => t.HasCheckConstraint("CK_Orders_STATUS",
                "Status IN ('pending', 'paid', 'processing', 'shipped', 'delivered', 'cancelled', 'refunded')"));

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(o => o.Seller)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId);

            builder.HasOne(o => o.Shipping)
                .WithOne(s => s.Order)
                .HasForeignKey<Shipping>(s => s.OrderId);

            builder.HasMany(o => o.Reviews)
                .WithOne(r => r.Order)
                .HasForeignKey(r => r.OrderId);
        }
    }
}
