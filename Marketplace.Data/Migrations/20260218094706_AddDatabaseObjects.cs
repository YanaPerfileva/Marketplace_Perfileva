using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Marketplace.Data.Migrations
{
    public partial class AddDatabaseObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER TRIGGER dbo.TR_ProductSkus_PriceChange
ON dbo.ProductSkus
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Price)
    BEGIN
        INSERT INTO dbo.PriceHistory (SkuId, OldPrice, NewPrice, ChangedAt)
        SELECT i.Id, d.Price, i.Price, GETDATE()
        FROM inserted i
        JOIN deleted d ON i.Id = d.Id
        WHERE ISNULL(i.Price, 0) != ISNULL(d.Price, 0);
    END
END
");

            migrationBuilder.Sql(@"
CREATE OR ALTER TRIGGER dbo.TR_ORDERITEMS_CheckStock
ON dbo.OrderItems
INSTEAD OF INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM inserted i
        JOIN dbo.ProductSkus s ON i.SkuId = s.Id
        WHERE s.Stock < i.Quantity
    )
    BEGIN
        THROW 51000, 'Недостаточно товара на складе', 1;
    END

    INSERT INTO dbo.OrderItems (OrderId, SkuId, Quantity, PriceAtTime, DiscountPercent)
    SELECT OrderId, SkuId, Quantity, PriceAtTime, DiscountPercent FROM inserted;

    UPDATE s
    SET Stock = s.Stock - i.Quantity,
        ReservedStock = s.ReservedStock + i.Quantity
    FROM dbo.ProductSkus s
    JOIN inserted i ON s.Id = i.SkuId;
END
");

            migrationBuilder.Sql(@"
CREATE OR ALTER TRIGGER dbo.TR_Orders_UpdateReservedStock
ON dbo.Orders
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Status) AND EXISTS (SELECT 1 FROM inserted WHERE Status IN ('delivered','cancelled'))
    BEGIN
        UPDATE s
        SET ReservedStock = ReservedStock - oi.Quantity
        FROM dbo.ProductSkus s
        JOIN dbo.OrderItems oi ON s.Id = oi.SkuId
        JOIN inserted i ON oi.OrderId = i.Id
        WHERE i.Status IN ('delivered','cancelled');
    END
END
");

            migrationBuilder.Sql("CREATE SEQUENCE dbo.SEQ_ORDER_NUMBER START WITH 1 INCREMENT BY 1;");

            migrationBuilder.Sql(@"
CREATE OR ALTER FUNCTION dbo.fn_CalculateFinalPrice
(
    @price DECIMAL(18,2),
    @DiscountPercent DECIMAL(5,2),
    @PromotionId INT = NULL
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @result DECIMAL(18,2) = @price;
    DECLARE @PromoDiscount DECIMAL(18,2) = 0;

    IF @DiscountPercent > 100 SET @DiscountPercent = 100;
    IF @DiscountPercent < 0 SET @DiscountPercent = 0;

    SET @result = @result * (1 - @DiscountPercent/100);

    IF @PromotionId IS NOT NULL
    BEGIN
        SELECT TOP 1 @PromoDiscount =
            CASE DiscountType
                WHEN 'percent' THEN @result * DiscountValue/100
                ELSE DiscountValue
            END
        FROM dbo.Promotions
        WHERE id = @PromotionId AND IsActive = 1 AND StartDate <= GETDATE() AND EndDate >= GETDATE();
        SET @result = @result - ISNULL(@PromoDiscount, 0);
    END

    IF @result < 0 SET @result = 0;
    RETURN @result;
END
");

            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE dbo.sp_GetUserOrders
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.Id, o.OrderNumber, o.Status, o.TotalPrice, o.CreatedAt,
           COUNT(oi.Id) as ItemsCount, s.StoreName as SellerName
    FROM dbo.Orders o
    JOIN dbo.Sellers s ON o.SellerId = s.Id
    LEFT JOIN dbo.OrderItems oi ON o.Id = oi.OrderId
    WHERE o.UserId = @UserId
    GROUP BY o.Id, o.OrderNumber, o.Status, o.TotalPrice, o.CreatedAt, s.StoreName
    ORDER BY o.CreatedAt DESC;
END
");

            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE dbo.sp_GetSellerProducts
    @SellerId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.Id, p.Name, p.Brand, c.Name as CategoryName, p.BasePrice, p.IsActive,
           p.ViewsCount, p.PurchaseCount, p.CreatedAt,
           COUNT(DISTINCT sku.Id) as SkuCount,
           SUM(sku.Stock) as TotalStock,
           MIN(sku.Price) as MinPrice,
           MAX(sku.Price) as MaxPrice
    FROM dbo.Products p
    JOIN dbo.Categories c ON p.CategoryId = c.Id
    LEFT JOIN dbo.ProductSkus sku ON p.Id = sku.ProductId
    WHERE p.SellerId = @SellerId
    GROUP BY p.Id, p.Name, p.Brand, c.Name, p.BasePrice, p.IsActive, p.ViewsCount, p.PurchaseCount, p.CreatedAt
    ORDER BY p.CreatedAt DESC;
END
");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS dbo.TR_ProductSkus_PriceChange;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS dbo.TR_ORDERITEMS_CheckStock;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS dbo.TR_Orders_UpdateReservedStock;");

            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_GetSellerProducts;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_GetUserOrders;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS dbo.fn_CalculateFinalPrice;");
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS dbo.SEQ_ORDER_NUMBER;");

            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Orders_USER_STATUS ON dbo.Orders;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_OrderItems_SKU ON dbo.OrderItems;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_ProductSkus_PRICE_STOCK ON dbo.ProductSkus;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Reviews_PRODUCT_RATING ON dbo.Reviews;");
        }
    }
}
