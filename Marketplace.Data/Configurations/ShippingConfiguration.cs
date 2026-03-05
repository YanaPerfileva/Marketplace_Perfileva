using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;

namespace Marketplace.Data.Configurations
{
    public class ShippingConfiguration : IEntityTypeConfiguration<Shipping>
    {
        public void Configure(EntityTypeBuilder<Shipping> builder)
        {
            builder.ToTable("Shipping");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.OrderId)
                .IsRequired();

            builder.Property(s => s.RecipientName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.RecipientPhone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.Address)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(s => s.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.PostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue("Россия");

            builder.Property(s => s.ShippingMethod)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion(
                    v => v.ToString(),
                    v => (ShippingMethod)Enum.Parse(typeof(ShippingMethod), v));

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion(
                    v => v.ToString(),
                    v => (ShippingStatus)Enum.Parse(typeof(ShippingStatus), v))
                .HasDefaultValue(ShippingStatus.pending);

            builder.Property(s => s.TrackingNumber)
                .HasMaxLength(100);

            builder.Property(s => s.EstimatedDelivery);

            builder.Property(s => s.DeliveredAt);

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETDATE()");

            builder.HasIndex(s => s.Status)
                .HasDatabaseName("IX_Shipping_STATUS");

            builder.HasIndex(s => s.TrackingNumber)
                .HasDatabaseName("IX_Shipping_TRACKING");

            builder.HasIndex(s => s.OrderId)
                .IsUnique()
                .HasDatabaseName("UQ_Shipping_ORDER");

            builder.ToTable(t => t.HasCheckConstraint("CK_Shipping_STATUS",
                 "Status IN ('pending', 'processing', 'shipped', 'delivered', 'returned')"));

            builder.ToTable(t => t.HasCheckConstraint("CK_Shipping_METHOD",
                 "ShippingMethod IN ('courier', 'pickup', 'post', 'express')"));

            builder.HasOne(s => s.Order)
                .WithOne(o => o.Shipping)
                .HasForeignKey<Shipping>(s => s.OrderId);
        }
    }
}
