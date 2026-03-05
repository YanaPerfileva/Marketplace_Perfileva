using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marketplace.Data.Migrations
{
    public partial class AddProductViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER VIEW dbo.v_active_products
AS
SELECT
    p.Id AS ProductId,
    p.Name AS ProductName,
    p.Brand,
    c.Name AS CategoryName,
    s.StoreName AS SellerName,
    CAST(ISNULL(MIN(ps.Price), 0) AS DECIMAL(18,2)) AS MinPrice,
    CAST(ISNULL(MAX(ps.Price), 0) AS DECIMAL(18,2)) AS MaxPrice,
    ISNULL(SUM(ps.Stock), 0) AS TotalStock
FROM dbo.Products p
JOIN dbo.Categories c ON c.Id = p.CategoryId
JOIN dbo.Sellers s ON s.Id = p.SellerId
LEFT JOIN dbo.ProductSkus ps ON ps.ProductId = p.Id AND ps.IsActive = 1
WHERE p.IsActive = 1
GROUP BY p.Id, p.Name, p.Brand, c.Name, s.StoreName;
");

            migrationBuilder.Sql(@"
CREATE OR ALTER VIEW dbo.v_popular_products
AS
SELECT
    p.Id AS Id,
    p.Name AS Name,
    p.Brand AS Brand,
    c.Name AS Category,
    COUNT(DISTINCT oi.OrderId) AS OrdersCount,
    ISNULL(SUM(oi.Quantity), 0) AS SoldCount,
    CAST(AVG(CAST(r.Rating AS DECIMAL(18,2))) AS DECIMAL(18,2)) AS AvgRating,
    ISNULL(SUM(ps.Stock), 0) AS AvailableStock
FROM dbo.Products p
JOIN dbo.Categories c ON c.Id = p.CategoryId
LEFT JOIN dbo.ProductSkus ps ON ps.ProductId = p.Id AND ps.IsActive = 1
LEFT JOIN dbo.OrderItems oi ON oi.SkuId = ps.Id
LEFT JOIN dbo.Reviews r ON r.ProductId = p.Id AND r.IsApproved = 1
GROUP BY p.Id, p.Name, p.Brand, c.Name;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.v_active_products;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS dbo.v_popular_products;");
        }
    }
}
