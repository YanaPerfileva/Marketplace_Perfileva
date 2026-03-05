-- ============================================================
--  Скрипт заполнения БД MarketplaceDB под текущую схему проекта
--  (учтены смешанные имена колонок: PascalCase)
-- ============================================================
USE MarketplaceDB;
GO

SET NOCOUNT ON;

-- ============================================================
-- ОЧИСТКА
-- ============================================================
PRINT N'НАЧАЛО ОЧИСТКИ ТАБЛИЦ...';

EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

DELETE FROM UserLogs;
DELETE FROM PriceHistory;
DELETE FROM PromotionProducts;
DELETE FROM Promotions;
DELETE FROM ProductImages;
DELETE FROM Favorites;
DELETE FROM Reviews;
DELETE FROM Shipping;
DELETE FROM Payments;
DELETE FROM OrderItems;
DELETE FROM Orders;
DELETE FROM CartItems;
DELETE FROM Carts;
DELETE FROM ProductSkus;
DELETE FROM Products;
DELETE FROM Categories;
DELETE FROM Sellers;
DELETE FROM Users;

EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';

DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Sellers', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('ProductSkus', RESEED, 0);
DBCC CHECKIDENT ('Carts', RESEED, 0);
DBCC CHECKIDENT ('CartItems', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('OrderItems', RESEED, 0);
DBCC CHECKIDENT ('Payments', RESEED, 0);
DBCC CHECKIDENT ('Shipping', RESEED, 0);
DBCC CHECKIDENT ('Reviews', RESEED, 0);
DBCC CHECKIDENT ('Favorites', RESEED, 0);
DBCC CHECKIDENT ('ProductImages', RESEED, 0);
DBCC CHECKIDENT ('Promotions', RESEED, 0);
DBCC CHECKIDENT ('UserLogs', RESEED, 0);

PRINT N'ОЧИСТКА ЗАВЕРШЕНА';
GO

-- ============================================================
-- Users / Sellers / Categories
-- ============================================================
INSERT INTO Users (Email, PasswordHash, FullName, Phone, Role, CreatedAt, IsActive) VALUES
('buyer1@mail.ru', 'password123', N'Иван Иванов', '+79000000001', 'buyer', DATEADD(day, -100, GETDATE()), 1),
('buyer2@mail.ru', 'password123', N'Петр Петров', '+79000000002', 'buyer', DATEADD(day, -95, GETDATE()), 1),
('buyer3@mail.ru', 'password123', N'Анна Сидорова', '+79000000003', 'buyer', DATEADD(day, -90, GETDATE()), 1),
('buyer4@mail.ru', 'password123', N'Ольга Орлова', '+79000000004', 'buyer', DATEADD(day, -85, GETDATE()), 1),
('buyer5@mail.ru', 'password123', N'Дмитрий Крылов', '+79000000005', 'buyer', DATEADD(day, -80, GETDATE()), 1),
('tech.store@mail.ru', 'password123', N'ТехноМир', '+74950000001', 'seller', DATEADD(day, -150, GETDATE()), 1),
('home.store@mail.ru', 'password123', N'ДомМаркет', '+74950000002', 'seller', DATEADD(day, -145, GETDATE()), 1),
('fashion.store@mail.ru', 'password123', N'Fashion Point', '+74950000003', 'seller', DATEADD(day, -140, GETDATE()), 1),
('admin@marketplace.ru', 'admin123', N'Администратор', '+74950000099', 'admin', DATEADD(day, -365, GETDATE()), 1);

INSERT INTO Sellers (UserId, StoreName, Description, Rating, TotalSales, Verified, CreatedAt) VALUES
(6, N'ТехноМир', N'Смартфоны и аксессуары', 4.8, 1250, 1, DATEADD(day, -150, GETDATE())),
(7, N'ДомМаркет', N'Товары для дома', 4.6, 850, 1, DATEADD(day, -145, GETDATE())),
(8, N'Fashion Point', N'Одежда и стиль', 4.7, 960, 1, DATEADD(day, -140, GETDATE()));

INSERT INTO Categories (Name, Description, ParentId, SortOrder, IsActive) VALUES
(N'Электроника', N'Гаджеты', NULL, 1, 1),
(N'Дом', N'Товары для дома', NULL, 2, 1),
(N'Одежда', N'Мужская и женская', NULL, 3, 1),
(N'Смартфоны', N'Смартфоны всех брендов', 1, 1, 1),
(N'Умный дом', N'Техника для дома', 2, 1, 1),
(N'Футболки', N'Базовые футболки', 3, 1, 1);
GO

-- ============================================================
-- Products + ProductSkus + ProductImages
-- ============================================================
DECLARE @TechSellerId INT = (SELECT TOP (1) Id FROM Sellers WHERE StoreName = N'ТехноМир' ORDER BY Id);
DECLARE @HomeSellerId INT = (SELECT TOP (1) Id FROM Sellers WHERE StoreName = N'ДомМаркет' ORDER BY Id);
DECLARE @FashionSellerId INT = (SELECT TOP (1) Id FROM Sellers WHERE StoreName = N'Fashion Point' ORDER BY Id);

DECLARE @PhonesCategoryId INT = (SELECT TOP (1) Id FROM Categories WHERE Name = N'Смартфоны' ORDER BY Id);
DECLARE @SmartHomeCategoryId INT = (SELECT TOP (1) Id FROM Categories WHERE Name = N'Умный дом' ORDER BY Id);
DECLARE @TshirtCategoryId INT = (SELECT TOP (1) Id FROM Categories WHERE Name = N'Футболки' ORDER BY Id);

DECLARE @i INT = 1;
WHILE @i <= 30
BEGIN
    DECLARE @SellerId INT = CASE
        WHEN @i % 3 = 1 THEN @TechSellerId
        WHEN @i % 3 = 2 THEN @HomeSellerId
        ELSE @FashionSellerId
    END;
    DECLARE @CategoryId INT = CASE
        WHEN @SellerId = @TechSellerId THEN @PhonesCategoryId
        WHEN @SellerId = @HomeSellerId THEN @SmartHomeCategoryId
        ELSE @TshirtCategoryId
    END;

    INSERT INTO Products (SellerId, CategoryId, Name, Brand, Description, BasePrice, IsActive, ViewsCount, PurchaseCount, CreatedAt)
    VALUES (
        @SellerId,
        @CategoryId,
        N'Товар #' + CAST(@i AS NVARCHAR(10)),
        CASE WHEN @SellerId = @TechSellerId THEN 'TechBrand' WHEN @SellerId = @HomeSellerId THEN 'HomeBrand' ELSE 'FashionBrand' END,
        N'Демо-описание товара #' + CAST(@i AS NVARCHAR(10)),
        1000 + @i * 150,
        1,
        @i * 12,
        @i * 2,
        DATEADD(day, -@i, GETDATE())
    );

    SET @i += 1;
END;

INSERT INTO ProductSkus (ProductId, SkuCode, Size, Color, ColorHex, Price, Stock, ReservedStock, IsActive, CreatedAt)
SELECT
    p.Id,
    'SKU-' + CAST(p.Id AS NVARCHAR(20)) + '-STD',
    CASE WHEN p.CategoryId = @TshirtCategoryId THEN 'M' ELSE 'One Size' END,
    CASE WHEN p.CategoryId = @TshirtCategoryId THEN N'Черный' ELSE N'Стандарт' END,
    CASE WHEN p.CategoryId = @TshirtCategoryId THEN '#000000' ELSE '#CCCCCC' END,
    p.BasePrice,
    20 + (p.Id % 40),
    0,
    1,
    DATEADD(day, -1, GETDATE())
FROM Products p;

INSERT INTO ProductImages (ProductId, SkuId, ImageUrl, IsMain, SortOrder, CreatedAt)
SELECT p.Id, NULL, '/images/products/' + CAST(p.Id AS NVARCHAR(20)) + '/main.jpg', 1, 0, GETDATE()
FROM Products p;
GO

-- ============================================================
-- Carts + CartItems
-- ============================================================
INSERT INTO Carts (UserId, CreatedAt, UpdatedAt, IsActive)
SELECT u.Id, DATEADD(day, -2, GETDATE()), GETDATE(), 1
FROM Users u
WHERE u.Role = 'buyer';

INSERT INTO CartItems (CartId, SkuId, Quantity, AddedAt)
SELECT TOP (50)
    c.Id,
    s.Id,
    1 + ABS(CHECKSUM(NEWID())) % 3,
    DATEADD(hour, -ABS(CHECKSUM(NEWID())) % 48, GETDATE())
FROM Carts c
CROSS JOIN ProductSkus s
ORDER BY NEWID();
GO

-- ============================================================
-- Orders + OrderItems + Payments + Shipping
-- ============================================================
DECLARE @TechSellerId INT = (SELECT TOP (1) Id FROM Sellers WHERE StoreName = N'ТехноМир' ORDER BY Id);
DECLARE @HomeSellerId INT = (SELECT TOP (1) Id FROM Sellers WHERE StoreName = N'ДомМаркет' ORDER BY Id);
DECLARE @FashionSellerId INT = (SELECT TOP (1) Id FROM Sellers WHERE StoreName = N'Fashion Point' ORDER BY Id);

DECLARE @Buyers TABLE (RowNum INT PRIMARY KEY, UserId INT NOT NULL);

INSERT INTO @Buyers (RowNum, UserId)
SELECT
    ROW_NUMBER() OVER (ORDER BY Id),
    Id
FROM Users
WHERE Role = 'buyer';

DECLARE @BuyersCount INT;
SELECT @BuyersCount = COUNT(*) FROM @Buyers;

IF @BuyersCount = 0
BEGIN
    RAISERROR(N'В seed-скрипте не найдено ни одного покупателя (Users.Role = ''buyer'').', 16, 1);
    RETURN;
END;

DECLARE @OrderIdx INT = 1;
WHILE @OrderIdx <= 15
BEGIN
    DECLARE @OrderUserId INT;
    SELECT @OrderUserId = b.UserId
    FROM @Buyers b
    WHERE b.RowNum = ((@OrderIdx - 1) % @BuyersCount) + 1;

    DECLARE @OrderSellerId INT;
    SET @OrderSellerId = CASE
        WHEN (@OrderIdx - 1) % 3 = 0 THEN @TechSellerId
        WHEN (@OrderIdx - 1) % 3 = 1 THEN @HomeSellerId
        ELSE @FashionSellerId
    END;

    INSERT INTO Orders (UserId, SellerId, OrderNumber, Status, TotalPrice, DiscountAmount, Comment, CreatedAt)
    VALUES (
        @OrderUserId,
        @OrderSellerId,
        'ORD-2026-' + RIGHT('0000' + CAST(@OrderIdx AS NVARCHAR(10)), 4),
        CASE WHEN @OrderIdx % 5 = 0 THEN 'cancelled' WHEN @OrderIdx % 3 = 0 THEN 'processing' ELSE 'delivered' END,
        0,
        0,
        N'Тестовый заказ #' + CAST(@OrderIdx AS NVARCHAR(10)),
        DATEADD(day, -@OrderIdx, GETDATE())
    );

    SET @OrderIdx += 1;
END;

INSERT INTO OrderItems (OrderId, SkuId, Quantity, PriceAtTime, DiscountPercent)
SELECT
    o.Id,
    s.Id,
    1 + (o.Id % 2),
    s.Price,
    CASE WHEN o.Id % 4 = 0 THEN 10 ELSE 0 END
FROM Orders o
CROSS APPLY (
    SELECT TOP (1) ps.Id, ps.Price
    FROM ProductSkus ps
    JOIN Products p ON p.Id = ps.ProductId
    WHERE p.SellerId = o.SellerId
    ORDER BY NEWID()
) s
WHERE o.Status <> 'cancelled';

UPDATE o
SET TotalPrice = x.Total,
    DiscountAmount = x.Discount
FROM Orders o
CROSS APPLY (
    SELECT
        CAST(ISNULL(SUM(oi.Quantity * oi.PriceAtTime), 0) AS DECIMAL(18,2)) AS Total,
        CAST(ISNULL(SUM(oi.Quantity * oi.PriceAtTime * oi.DiscountPercent / 100.0), 0) AS DECIMAL(18,2)) AS Discount
    FROM OrderItems oi
    WHERE oi.OrderId = o.Id
) x;

-- Payments: в текущей схеме Status и PaymentMethod - int enum
-- PaymentStatus: pending=0, processing=1, paid=2, failed=3, refunded=4
-- PaymentMethod: card=0, cash=1, sbp=2, wallet=3, crypto=4
INSERT INTO Payments (OrderId, Amount, Status, PaymentMethod, TransactionId, PaidAt, CreatedAt)
SELECT
    o.Id,
    o.TotalPrice - o.DiscountAmount,
    CASE WHEN o.Status = 'cancelled' THEN 4 ELSE 2 END,
    ABS(CHECKSUM(NEWID())) % 4,
    'TXN-' + FORMAT(o.CreatedAt, 'yyyyMMdd') + '-' + CAST(o.Id AS NVARCHAR(20)),
    CASE WHEN o.Status = 'cancelled' THEN NULL ELSE DATEADD(hour, 1, o.CreatedAt) END,
    o.CreatedAt
FROM Orders o
WHERE o.TotalPrice > 0;

INSERT INTO Shipping (OrderId, RecipientName, RecipientPhone, Address, City, PostalCode, Country, ShippingMethod, Status, TrackingNumber, EstimatedDelivery, DeliveredAt, CreatedAt)
SELECT
    o.Id,
    u.FullName,
    ISNULL(u.Phone, '+79000000000'),
    N'ул. Тестовая, д. ' + CAST(o.Id AS NVARCHAR(10)),
    N'Москва',
    '101000',
    N'Россия',
    CASE WHEN o.Id % 4 = 0 THEN 'pickup' ELSE 'courier' END,
    CASE WHEN o.Status = 'delivered' THEN 'delivered' ELSE 'processing' END,
    'TRK-' + CAST(o.Id AS NVARCHAR(20)),
    DATEADD(day, 3, o.CreatedAt),
    CASE WHEN o.Status = 'delivered' THEN DATEADD(day, 2, o.CreatedAt) ELSE NULL END,
    o.CreatedAt
FROM Orders o
JOIN Users u ON u.Id = o.UserId;
GO

-- ============================================================
-- Reviews + Favorites + Promotions + PromotionProducts
-- ============================================================
INSERT INTO Reviews (UserId, ProductId, OrderId, Rating, Title, Comment, IsVerifiedPurchase, IsApproved, HelpfulCount, CreatedAt)
SELECT
    o.UserId,
    p.Id,
    o.Id,
    4 + (o.Id % 2),
    N'Хороший товар',
    N'Покупкой доволен',
    1,
    1,
    o.Id % 10,
    DATEADD(day, 1, o.CreatedAt)
FROM Orders o
JOIN OrderItems oi ON oi.OrderId = o.Id
JOIN ProductSkus sku ON sku.Id = oi.SkuId
JOIN Products p ON p.Id = sku.ProductId
WHERE o.Status = 'delivered'
  AND NOT EXISTS (
      SELECT 1
      FROM Reviews r
      WHERE r.UserId = o.UserId AND r.ProductId = p.Id AND r.OrderId = o.Id
  );

INSERT INTO Favorites (UserId, ProductId, CreatedAt)
SELECT DISTINCT
    u.Id,
    p.Id,
    DATEADD(day, -ABS(CHECKSUM(NEWID())) % 30, GETDATE())
FROM Users u
CROSS JOIN Products p
WHERE u.Role = 'buyer'
  AND p.Id <= 20
  AND ABS(CHECKSUM(NEWID())) % 100 < 25;

INSERT INTO Promotions (Name, Description, DiscountType, DiscountValue, StartDate, EndDate, IsActive, CreatedAt) VALUES
(N'Сезонная скидка', N'Скидки на популярные товары', 'percent', 15, DATEADD(day, -3, GETDATE()), DATEADD(day, 10, GETDATE()), 1, GETDATE()),
(N'Фиксированная скидка', N'Скидка в рублях', 'fixed', 500, DATEADD(day, -1, GETDATE()), DATEADD(day, 14, GETDATE()), 1, GETDATE());

DECLARE @SeasonPromotionId INT = (SELECT TOP (1) Id FROM Promotions WHERE Name = N'Сезонная скидка' ORDER BY Id DESC);
DECLARE @FixedPromotionId INT = (SELECT TOP (1) Id FROM Promotions WHERE Name = N'Фиксированная скидка' ORDER BY Id DESC);

INSERT INTO PromotionProducts (PromotionId, ProductId)
SELECT TOP (10) @SeasonPromotionId, p.Id
FROM Products p
ORDER BY NEWID();

INSERT INTO PromotionProducts (PromotionId, ProductId)
SELECT TOP (5) @FixedPromotionId, p.Id
FROM Products p
WHERE p.BasePrice > 3000
ORDER BY NEWID();
GO

-- ============================================================
-- PriceHistory + UserLogs
-- ============================================================
INSERT INTO PriceHistory (SkuId, OldPrice, NewPrice, ChangedBy, ChangedAt)
SELECT
    s.Id,
    s.Price + 200,
    s.Price,
    (SELECT TOP (1) Id FROM Users WHERE Email = 'admin@marketplace.ru' ORDER BY Id),
    DATEADD(day, -ABS(CHECKSUM(NEWID())) % 20, GETDATE())
FROM ProductSkus s
WHERE ABS(CHECKSUM(NEWID())) % 100 < 30;

INSERT INTO UserLogs (UserId, Action, EntityType, EntityId, Details, IpAddress, CreatedAt)
SELECT
    u.Id,
    CASE v.n WHEN 0 THEN 'LOGIN' WHEN 1 THEN 'VIEW_PRODUCT' WHEN 2 THEN 'ADD_TO_CART' ELSE 'CREATE_ORDER' END,
    CASE v.n WHEN 0 THEN NULL WHEN 1 THEN 'PRODUCT' WHEN 2 THEN 'CART' ELSE 'ORDER' END,
    CASE WHEN v.n = 1 THEN p.Id WHEN v.n = 2 THEN c.Id WHEN v.n = 3 THEN o.Id ELSE NULL END,
    CONCAT('{"source":"seed","event":', v.n, '}'),
    CONCAT('10.0.', ABS(CHECKSUM(NEWID())) % 255, '.', ABS(CHECKSUM(NEWID())) % 255),
    DATEADD(hour, -v.n, GETDATE())
FROM Users u
CROSS JOIN (VALUES (0), (1), (2), (3)) v(n)
LEFT JOIN Products p ON p.Id = ((u.Id + v.n) % 30) + 1 AND v.n = 1
LEFT JOIN Carts c ON c.UserId = u.Id AND v.n = 2
LEFT JOIN Orders o ON o.UserId = u.Id AND v.n = 3
WHERE u.Role <> 'admin';
GO

-- ============================================================
-- ФИНАЛЬНАЯ ПРОВЕРКА
-- ============================================================
SELECT 'Users' AS TableName, COUNT(*) AS RowsCount FROM Users
UNION ALL SELECT 'Sellers', COUNT(*) FROM Sellers
UNION ALL SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL SELECT 'Products', COUNT(*) FROM Products
UNION ALL SELECT 'ProductSkus', COUNT(*) FROM ProductSkus
UNION ALL SELECT 'ProductImages', COUNT(*) FROM ProductImages
UNION ALL SELECT 'Carts', COUNT(*) FROM Carts
UNION ALL SELECT 'CartItems', COUNT(*) FROM CartItems
UNION ALL SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL SELECT 'OrderItems', COUNT(*) FROM OrderItems
UNION ALL SELECT 'Payments', COUNT(*) FROM Payments
UNION ALL SELECT 'Shipping', COUNT(*) FROM Shipping
UNION ALL SELECT 'Reviews', COUNT(*) FROM Reviews
UNION ALL SELECT 'Favorites', COUNT(*) FROM Favorites
UNION ALL SELECT 'Promotions', COUNT(*) FROM Promotions
UNION ALL SELECT 'PromotionProducts', COUNT(*) FROM PromotionProducts
UNION ALL SELECT 'PriceHistory', COUNT(*) FROM PriceHistory
UNION ALL SELECT 'UserLogs', COUNT(*) FROM UserLogs
ORDER BY TableName;
GO
