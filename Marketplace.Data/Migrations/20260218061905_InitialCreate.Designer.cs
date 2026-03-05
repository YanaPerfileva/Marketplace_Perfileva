
using System;
using Marketplace.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Marketplace.Data.Migrations
{
    [DbContext(typeof(MarketplaceContext))]
    [Migration("20260218061905_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Marketplace.Data.Entities.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true)
                        .HasColumnName("IsActive");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductSkuId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("ProductSkuId");

                    b.HasIndex("UserId")
                        .HasDatabaseName("IX_Carts_USER")
                        .HasFilter("IsActive = 1");

                    b.ToTable("Carts", (string)null);
                });

            modelBuilder.Entity("Marketplace.Data.Entities.CartItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AddedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("SkuId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("SkuId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("ImageUrl");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true)
                        .HasColumnName("IsActive");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int")
                        .HasColumnName("ParentId");

                    b.Property<int>("SortOrder")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("SortOrder");

                    b.HasKey("Id");

                    b.HasIndex("ParentId")
                        .HasDatabaseName("IX_Categories_PARENT");

                    b.HasIndex("SortOrder")
                        .HasDatabaseName("IX_Categories_SORT");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Favorite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("Favorites");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Order", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Seller", "Seller")
        .WithMany("Orders")
        .HasForeignKey("SellerId")
        .OnDelete(DeleteBehavior.NoAction)
        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Seller");

                    b.Navigation("User");
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("Comment");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<decimal>("DiscountAmount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(18,2)")
                        .HasDefaultValue(0m)
                        .HasColumnName("DiscountAmount");

                    b.Property<decimal>("FinalPrice")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("FinalPrice")
                        .HasComputedColumnSql("[TotalPrice] - [DiscountAmount]", true);

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("OrderNumber");

                    b.Property<int>("SellerId")
                        .HasColumnType("int")
                        .HasColumnName("SellerId");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("pending")
                        .HasColumnName("Status");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("TotalPrice");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt")
                        .IsDescending()
                        .HasDatabaseName("IX_Orders_CREATED");

                    b.HasIndex("OrderNumber")
                        .IsUnique()
                        .HasDatabaseName("UQ_ORDER_NUMBER");

                    b.HasIndex("SellerId")
                        .HasDatabaseName("IX_Orders_SELLER");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_Orders_STATUS");

                    b.HasIndex("UserId")
                        .HasDatabaseName("IX_Orders_USER");

                    b.HasIndex("UserId", "Status")
                        .HasDatabaseName("IX_Orders_USER_STATUS");

                    b.ToTable("Orders", null, t =>
                        {
                            t.HasCheckConstraint("CK_Orders_DISCOUNT", "DiscountAmount >= 0");

                            t.HasCheckConstraint("CK_Orders_PRICE", "TotalPrice >= 0");

                            t.HasCheckConstraint("CK_Orders_STATUS", "Status IN ('pending', 'paid', 'processing', 'shipped', 'delivered', 'cancelled', 'refunded')");
                        });
                });

            modelBuilder.Entity("Marketplace.Data.Entities.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("DiscountPercent")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<decimal>("PriceAtTime")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("SkuId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SkuId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PaidAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("PaymentDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PaymentMethod")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("int");

                    b.Property<string>("TransactionId")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.PriceHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ChangedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ChangedBy")
                        .HasColumnType("int");

                    b.Property<decimal>("NewPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("OldPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("SkuId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChangedBy");

                    b.HasIndex("SkuId");

                    b.ToTable("PriceHistory");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("BasePrice")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("BasePrice");

                    b.Property<string>("Brand")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int")
                        .HasColumnName("CategoryId");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true)
                        .HasColumnName("IsActive");

                    b.Property<string>("MainImageUrl")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("MainImageUrl");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("PurchaseCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("PurchaseCount");

                    b.Property<int>("SellerId")
                        .HasColumnType("int")
                        .HasColumnName("SellerId");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt");

                    b.Property<int>("ViewsCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("ViewsCount");

                    b.HasKey("Id");

                    b.HasIndex("BasePrice")
                        .HasDatabaseName("IX_Products_PRICE");

                    b.HasIndex("CategoryId")
                        .HasDatabaseName("IX_Products_CATEGORY");

                    b.HasIndex("CreatedAt")
                        .IsDescending()
                        .HasDatabaseName("IX_Products_CREATED");

                    b.HasIndex("PurchaseCount")
                        .IsDescending()
                        .HasDatabaseName("IX_Products_POPULAR");

                    b.HasIndex("SellerId")
                        .HasDatabaseName("IX_Products_SELLER");

                    b.ToTable("Products", null, t =>
                        {
                            t.HasCheckConstraint("CK_Products_PRICE", "BasePrice >= 0");
                        });
                });

            modelBuilder.Entity("Marketplace.Data.Entities.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsMain")
                        .HasColumnType("bit");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("SkuId")
                        .HasColumnType("int");

                    b.Property<int>("SortOrder")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SkuId");

                    b.ToTable("ProductImages");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.ProductSku", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Color")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ColorHex")
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("ReservedStock")
                        .HasColumnType("int");

                    b.Property<string>("Size")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("SkuCode")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductSkus");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Promotion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("DiscountType")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("DiscountType");

                    b.Property<decimal>("DiscountValue")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("DiscountValue");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("EndDate");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true)
                        .HasColumnName("IsActive");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("StartDate", "EndDate")
                        .HasDatabaseName("IX_Promotions_DATES");

                    b.ToTable("Promotions", null, t =>
                        {
                            t.HasCheckConstraint("CK_Promotions_DATE", "StartDate < EndDate");

                            t.HasCheckConstraint("CK_Promotions_TYPE", "DiscountType IN ('percent', 'fixed')");
                        });
                });

            modelBuilder.Entity("Marketplace.Data.Entities.PromotionProduct", b =>
                {
                    b.Property<int>("PromotionId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("PromotionId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("PromotionProducts", (string)null);
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("HelpfulCount")
                        .HasColumnType("int");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerifiedPurchase")
                        .HasColumnType("bit");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Seller", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("Description");

                    b.Property<string>("LogoUrl")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("LogoUrl");

                    b.Property<decimal?>("Rating")
                        .HasColumnType("decimal(3,2)")
                        .HasColumnName("Rating");

                    b.Property<string>("StoreName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasColumnName("StoreName");

                    b.Property<int>("TotalSales")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("TotalSales");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserId");

                    b.Property<bool>("Verified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("Verified");

                    b.HasKey("Id");

                    b.HasIndex("Rating")
                        .IsDescending()
                        .HasDatabaseName("IX_Sellers_RATING");

                    b.HasIndex("StoreName")
                        .IsUnique()
                        .HasDatabaseName("UQ_SELLER_STORE");

                    b.HasIndex("UserId");

                    b.HasIndex("Verified")
                        .HasDatabaseName("IX_Sellers_VERIFIED");

                    b.ToTable("Sellers", (string)null);
                });

            modelBuilder.Entity("Marketplace.Data.Entities.SellerStatistics", b =>
                {
                    b.Property<decimal>("AvgRating")
                        .HasColumnType("decimal(5,2)");

                    b.Property<int>("ItemsSold")
                        .HasColumnType("int");

                    b.Property<int>("SellerId")
                        .HasColumnType("int");

                    b.Property<string>("StoreName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalOrders")
                        .HasColumnType("int");

                    b.Property<int>("TotalProducts")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalRevenue")
                        .HasColumnType("decimal(18,2)");

                    b.ToTable((string)null);

                    b.ToView("vw_SellerStatistics", (string)null);
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Shipping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("Address");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("City");

                    b.Property<string>("Country")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasDefaultValue("Россия")
                        .HasColumnName("Country");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<DateTime?>("DeliveredAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("DeliveredAt");

                    b.Property<DateTime?>("EstimatedDelivery")
                        .HasColumnType("datetime2")
                        .HasColumnName("EstimatedDelivery");

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("OrderId");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("PostalCode");

                    b.Property<string>("RecipientName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasColumnName("RecipientName");

                    b.Property<string>("RecipientPhone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("RecipientPhone");

                    b.Property<string>("ShippingMethod")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("ShippingMethod");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("pending")
                        .HasColumnName("Status");

                    b.Property<string>("TrackingNumber")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("TrackingNumber");

                    b.HasKey("Id");

                    b.HasIndex("OrderId")
                        .IsUnique()
                        .HasDatabaseName("UQ_Shipping_ORDER");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_Shipping_STATUS");

                    b.HasIndex("TrackingNumber")
                        .HasDatabaseName("IX_Shipping_TRACKING");

                    b.ToTable("Shipping", null, t =>
                        {
                            t.HasCheckConstraint("CK_Shipping_METHOD", "ShippingMethod IN ('courier', 'pickup', 'post', 'express')");

                            t.HasCheckConstraint("CK_Shipping_STATUS", "Status IN ('pending', 'processing', 'shipped', 'delivered', 'returned')");
                        });
                });

            modelBuilder.Entity("Marketplace.Data.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedAt")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)")
                        .HasColumnName("FullName");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true)
                        .HasColumnName("IsActive");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("datetime2")
                        .HasColumnName("LastLogin");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("PasswordHash");

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("Phone");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("Role");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("UQ_USER_EMAIL");

                    b.HasIndex("Role")
                        .HasDatabaseName("IX_Users_ROLE");

                    b.ToTable("Users", null, t =>
                        {
                            t.HasCheckConstraint("CK_Users_ROLE", "Role IN ('buyer', 'seller', 'admin')");
                        });
                });

            modelBuilder.Entity("Marketplace.Data.Entities.UserLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EntityId")
                        .HasColumnType("int");

                    b.Property<string>("EntityType")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(45)
                        .HasColumnType("nvarchar(45)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogs");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Cart", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Product", null)
                        .WithMany("Carts")
                        .HasForeignKey("ProductId");

                    b.HasOne("Marketplace.Data.Entities.ProductSku", null)
                        .WithMany("Carts")
                        .HasForeignKey("ProductSkuId");

                    b.HasOne("Marketplace.Data.Entities.User", "User")
                        .WithMany("Carts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.CartItem", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Cart", "Cart")
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.ProductSku", "ProductSku")
                        .WithMany()
                        .HasForeignKey("SkuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("ProductSku");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Category", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Category", "Parent")
                        .WithMany("SubCategories")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Favorite", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Product", "Product")
                        .WithMany("Favorites")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.User", "User")
                        .WithMany("Favorites")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Order", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Seller", "Seller")
                        .WithMany("Orders")
                        .HasForeignKey("SellerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Seller");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.OrderItem", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.Product", null)
                        .WithMany("OrderItems")
                        .HasForeignKey("ProductId");

                    b.HasOne("Marketplace.Data.Entities.ProductSku", "ProductSku")
                        .WithMany("OrderItems")
                        .HasForeignKey("SkuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("ProductSku");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Payment", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Order", "Order")
                        .WithOne("Payment")
                        .HasForeignKey("Marketplace.Data.Entities.Payment", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.PriceHistory", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.User", "ChangedByUser")
                        .WithMany("PriceHistories")
                        .HasForeignKey("ChangedBy");

                    b.HasOne("Marketplace.Data.Entities.ProductSku", "ProductSku")
                        .WithMany("PriceHistories")
                        .HasForeignKey("SkuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ChangedByUser");

                    b.Navigation("ProductSku");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Product", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.Seller", "Seller")
                        .WithMany("Products")
                        .HasForeignKey("SellerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Seller");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.ProductImage", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Category", null)
                        .WithMany("ProductImages")
                        .HasForeignKey("CategoryId");

                    b.HasOne("Marketplace.Data.Entities.Product", "Product")
                        .WithMany("ProductImages")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.ProductSku", "ProductSku")
                        .WithMany("ProductImages")
                        .HasForeignKey("SkuId");

                    b.Navigation("Product");

                    b.Navigation("ProductSku");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.ProductSku", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Product", "Product")
                        .WithMany("Skus")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.PromotionProduct", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Product", "Product")
                        .WithMany("PromotionProducts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.Promotion", "Promotion")
                        .WithMany("PromotionProducts")
                        .HasForeignKey("PromotionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Promotion");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Review", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Order", "Order")
                        .WithMany("Reviews")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.Product", "Product")
                        .WithMany("Reviews")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Marketplace.Data.Entities.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Seller", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.User", "User")
                        .WithMany("Sellers")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Shipping", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.Order", "Order")
                        .WithOne("Shipping")
                        .HasForeignKey("Marketplace.Data.Entities.Shipping", "OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.UserLog", b =>
                {
                    b.HasOne("Marketplace.Data.Entities.User", "User")
                        .WithMany("UserLogs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("User");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Cart", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Category", b =>
                {
                    b.Navigation("ProductImages");

                    b.Navigation("Products");

                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Order", b =>
                {
                    b.Navigation("OrderItems");

                    b.Navigation("Payment");

                    b.Navigation("Reviews");

                    b.Navigation("Shipping");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Product", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Favorites");

                    b.Navigation("OrderItems");

                    b.Navigation("ProductImages");

                    b.Navigation("PromotionProducts");

                    b.Navigation("Reviews");

                    b.Navigation("Skus");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.ProductSku", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("OrderItems");

                    b.Navigation("PriceHistories");

                    b.Navigation("ProductImages");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Promotion", b =>
                {
                    b.Navigation("PromotionProducts");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.Seller", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("Marketplace.Data.Entities.User", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Favorites");

                    b.Navigation("Orders");

                    b.Navigation("PriceHistories");

                    b.Navigation("Reviews");

                    b.Navigation("Sellers");

                    b.Navigation("UserLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
