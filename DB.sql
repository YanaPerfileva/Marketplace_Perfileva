-- ============================================================
-- 1. ПОЛЬЗОВАТЕЛИ (USERS) - 16 шт - ВЫРОВНЕНЫ ПОЛЯ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ USERS...';

INSERT INTO USERS (email, PasswordHash, FullName, phone, role, createdat, isactive) VALUES
-- Покупатели (id 1-10)
('ivan.ivanov@mail.ru','password123','Иванов Иван Петрович','+79001234567','buyer', DATEADD(day, -150, GETDATE()), 1),
('petr.sidorov@yandex.ru','qwerty123','Петр Сидоров','+79017654321',  'buyer', DATEADD(day, -140, GETDATE()), 1),
('anna.smirnova@gmail.com','anna2024', 'Анна Смирнова', '+79161234567', 'buyer', DATEADD(day, -130, GETDATE()), 1),
('elena.kozlova@bk.ru','elena123','Елена Козлова','+79167654321','buyer', DATEADD(day, -120, GETDATE()), 1),
('dmitry.volkov@mail.ru','dmitry2024','Дмитрий Волков', '+79251234567', 'buyer', DATEADD(day, -110, GETDATE()), 1),
('olga.morozova@yandex.ru','olga123','Ольга Морозова','+79257654321',  'buyer', DATEADD(day, -100, GETDATE()), 1),
('alexey.pavlov@gmail.com','alexey2024','Алексей Павлов','+79361234567','buyer', DATEADD(day, -90, GETDATE()), 1),
('tatiana.nikolaeva@bk.ru','tatiana123','Татьяна Николаева','+79367654321', 'buyer', DATEADD(day, -80, GETDATE()), 1),
('andrey.romanov@mail.ru','andrey2024','Андрей Романов','+79451234567',  'buyer', DATEADD(day, -70, GETDATE()), 1),
('marina.sokolova@yandex.ru','marina123', 'Марина Соколова','+79457654321', 'buyer', DATEADD(day, -60, GETDATE()), 1),
-- Продавцы (id 11-15)
('tech.store@mail.ru', 'tech2024', 'ТехноМир', '+74951234567', 'seller',DATEADD(day, -200, GETDATE()), 1),
('fashion.house@yandex.ru','fashion2024', 'Модный Дом', '+74957654321', 'seller',DATEADD(day, -190, GETDATE()), 1),
('home.garden@gmail.com', 'home2024', 'Дом и Сад', '+74961234567',  'seller',DATEADD(day, -180, GETDATE()), 1),
('sport.master@bk.ru', 'sport2024', 'СпортМастер', '+74967654321','seller',DATEADD(day, -170, GETDATE()), 1),
('book.world@mail.ru', 'book2024','Книжный Мир','+74971234567','seller',DATEADD(day, -160, GETDATE()), 1),
-- Админ (id 16)
('admin@marketplace.ru', 'admin123', 'Администратор','+74997654321','admin', DATEADD(day, -365, GETDATE()), 1);

PRINT '  ✓ USERS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 2. ПРОДАВЦЫ (SELLERS) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ SELLERS...';

INSERT INTO SELLERS (userid, storename, description, rating, totalsales, verified, createdat) VALUES
(1, 'ТехноМир',                  'Все для цифровой жизни: смартфоны, ноутбуки, аксессуары', 4.8, 1250, 1, DATEADD(day, -200, GETDATE())),
(2, 'Модный Дом',                'Модная одежда и аксессуары для всей семьи',               4.6, 3450, 1, DATEADD(day, -190, GETDATE())),
(3, 'Дом и Сад',                 'Товары для уюта в вашем доме и на даче',                   4.7,  890, 1, DATEADD(day, -180, GETDATE())),
(4, 'СпортМастер',               'Все для спорта и активного отдыха',                         4.5, 2340, 1, DATEADD(day, -170, GETDATE())),
(5, 'Книжный Мир',               'Книги на любой вкус: художественная литература, учебники', 4.9, 5670, 1, DATEADD(day, -160, GETDATE()));

PRINT '  ✓ SELLERS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 3. КАТЕГОРИИ (CATEGORIES) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ CATEGORIES...';

INSERT INTO CATEGORIES (name, description, parentid, sortorder, isactive) VALUES
('Электроника', 'Бытовая электроника и гаджеты', NULL, 1, 1),
('Одежда',      'Модная одежда для всех',       NULL, 2, 1),
('Дом и сад',   'Товары для дома и дачи',       NULL, 3, 1),
('Спорт',       'Спортивные товары и инвентарь',NULL, 4, 1),
('Книги',       'Литература всех жанров',       NULL, 5, 1),
('Смартфоны',   'Мобильные телефоны',           1,   1, 1),
('Ноутбуки',    'Портативные компьютеры',       1,   2, 1),
('Наушники',    'Аудио-гарнитура',              1,   3, 1),
('Мужская одежда','Одежда для мужчин',         2,   1, 1),
('Женская одежда','Одежда для женщин',         2,   2, 1),
('Мебель',      'Мебель для дома',              3,   1, 1),
('Посуда',      'Кухонная утварь',              3,   2, 1),
('Apple',       'Техника Apple',                6,   1, 1),
('Samsung',     'Техника Samsung',              6,   2, 1),
('Xiaomi',      'Техника Xiaomi',               6,   3, 1),
('Футболки',    'Мужские футболки',             9,   1, 1),
('Джинсы',      'Мужские джинсы',               9,   2, 1),
('Платья',      'Женские платья',               10,  1, 1),
('Блузки',      'Женские блузки',               10,  2, 1);

PRINT '  ✓ CATEGORIES: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 4. ПРОДУКТЫ (PRODUCTS) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ PRODUCTS...';

INSERT INTO PRODUCTS (sellerid, categoryid, name, brand, description, baseprice, isactive, viewscount, purchasecount, createdat) VALUES
(1, 13, 'iPhone 15 Pro',           'Apple', 'Флагманский смартфон 2024',              89990, 1, 1250, 45, DATEADD(day, -30, GETDATE())),
(1, 14, 'Samsung Galaxy S24',      'Samsung','Android флагман',                      79990, 1,  980, 38, DATEADD(day, -25, GETDATE())),
(1, 15, 'Xiaomi 14',               'Xiaomi','Мощный камерофон',                     59990, 1,  750, 29, DATEADD(day, -20, GETDATE())),
(2, 16, 'Футболка хлопковая',      'Nike',  'Дышащая ткань, классический крой',       1990,  1,  450, 120, DATEADD(day, -15, GETDATE())),
(2, 17, 'Джинсы классические',     'Levi''s','Прочный деним, прямой крой',            4990,  1,  320,  85, DATEADD(day, -12, GETDATE())),
(2, 18, 'Платье летнее',           'Zara',  'Легкое шифон, размерный ряд 42-50',      3490,  1,  280,  65, DATEADD(day, -10, GETDATE())),
(3, 11, 'Стол обеденный',          'IKEA',  'Дерево массив, 4 стула в комплекте',     15990, 1,  180,  25, DATEADD(day, -40, GETDATE())),
(4, 8,  'Наушники JBL',            'JBL',   'Bluetooth 5.0, шумоподавление',          4990,  1,  650,  95, DATEADD(day, -35, GETDATE())),
(5, 5,  'Война и мир',             'Эксмо', 'Толстой Л.Н., комплект 4 тома',          1290,  1,  210,  67, DATEADD(day, -50, GETDATE()));

PRINT '  ✓ PRODUCTS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 5. АРТИКУЛЫ (PRODUCT_SKU) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ PRODUCT_SKU...';

INSERT INTO ProductSkus(ProductId, SkuCode, Size, Color, ColorHex, Price, Stock, ReservedStock, IsActive, CreatedAt) 
VALUES
(1, 'SKU-IPH15-BLK', 'One Size', 'Черный',  '#000000', 89990, 15, 2, 1, GETDATE()),
(1, 'SKU-IPH15-SVR', 'One Size', 'Серебро', '#C0C0C0', 89990,  8, 1, 1, GETDATE()),
(1, 'SKU-IPH15-BLU', 'One Size', 'Голубой', '#4169E1', 89990, 12, 3, 1, GETDATE()),
(2, 'SKU-S24-BLK',   'One Size', 'Черный',  '#000000', 79990, 22, 5, 1, GETDATE()),
(2, 'SKU-S24-VIO',   'One Size', 'Фиолет',  '#9370DB', 79990, 18, 4, 1, GETDATE()),
(4, 'SKU-NIKE-S-BLK','S',        'Черный',  '#000000', 1990,  25, 3, 1, GETDATE()),
(4, 'SKU-NIKE-M-WHT','M',        'Белый',   '#FFFFFF', 1790,  18, 2, 1, GETDATE()),
(4, 'SKU-NIKE-L-RED','L',        'Красный', '#FF0000', 2290,  12, 1, 1, GETDATE());

PRINT '  ✓ PRODUCT_SKU: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO



-- 6. ИЗОБРАЖЕНИЯ (PRODUCT_IMAGES) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ PRODUCT_IMAGES...';

INSERT INTO ProductImages (ProductId, SkuId, ImageUrl, IsMain, SortOrder, CreatedAt, CategoryId) 
VALUES

(1, 1, '/images/products/1/main.jpg', 1, 0, GETDATE(), 13),
(1, 2, '/images/products/1/1.jpg',    0, 1, GETDATE(), 13),
(1, 3, '/images/products/1/2.jpg',    0, 2, GETDATE(), 13),

(2, 4, '/images/products/2/main.jpg', 1, 0, GETDATE(), 14),
(2, 5, '/images/products/2/1.jpg',    0, 1, GETDATE(), 14),

(4, 6, '/images/products/4/main.jpg', 1, 0, GETDATE(), 16),
(4, 7, '/images/products/4/1.jpg',   0, 1, GETDATE(), 16),
(4, 8, '/images/products/4/2.jpg',   0, 2, GETDATE(), 16);

PRINT '  ✓ PRODUCT_IMAGES: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO
-- ============================================================
-- 7. КОРЗИНЫ (CARTS) - ИСПРАВЛЕНО
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ CARTS...';

INSERT INTO Carts (UserId, CreatedAt, UpdatedAt, IsActive) 
VALUES
(1, DATEADD(day, -3, GETDATE()), GETDATE(), 1),
(2, DATEADD(day, -5, GETDATE()), GETDATE(), 1),
(3, DATEADD(day, -1, GETDATE()), GETDATE(), 1),
(4, DATEADD(day, -7, GETDATE()), GETDATE(), 1);

PRINT '  ✓ CARTS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 8. ТОВАРЫ В КОРЗИНАХ (CART_ITEMS) - ИСПРАВЛЕНО
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ CART_ITEMS...';

INSERT INTO CartItems (CartId, SkuId, Quantity, AddedAt) VALUES
(1, 1, 1, DATEADD(hour, -2,  GETDATE())),
(2, 4, 2, DATEADD(hour, -5,  GETDATE())),
(3, 6, 1, DATEADD(hour, -1,  GETDATE())),
(4, 2, 1, DATEADD(hour, -10, GETDATE()));

PRINT '  ✓ CART_ITEMS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 9. ЗАКАЗЫ (ORDERS) - camelCase названия столбцов
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ ORDERS...';

INSERT INTO ORDERS (userId, sellerId, orderNumber, status, totalPrice, discountAmount, comment, createdAt) VALUES
(1,  1, 'ORD-2026-0001', 'delivered',   91980, 0,   'Доставка на дом',     DATEADD(day, -10, GETDATE())),
(2,  2, 'ORD-2026-0002', 'processing',  5280,  400, 'Безналичная оплата',  DATEADD(day, -8,  GETDATE())),
(3,  1, 'ORD-2026-0003', 'shipped',     89990, 0,   'Срочная доставка',   DATEADD(day, -5,  GETDATE())),
(4,  4, 'ORD-2026-0004', 'delivered',   8990,  0,   'Обычная доставка',   DATEADD(day, -3,  GETDATE()));

PRINT '  ✓ ORDERS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 10. ПОЗИЦИИ ЗАКАЗОВ (ORDER ITEMS) - camelCase
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ ORDER ITEMS...';

INSERT INTO ORDERITEMS (orderId, skuId, quantity, priceAtTime, discountPercent) VALUES
(1, 1, 1, 89990, 0),
(1, 6, 1, 1990,  0),
(2, 7, 2, 1790,  10),
(3, 1, 1, 89990, 0),
(4, 8, 1, 4990,  0);

PRINT '  ✓ ORDER ITEMS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 11. ПЛАТЕЖИ (PAYMENTS) - camelCase
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ PAYMENTS...';

INSERT INTO Payments (OrderId, Amount, Status, PaymentMethod, TransactionId, PaymentDetails, PaidAt, CreatedAt) VALUES
(1, 91980, 1, 1, 'TXN-20260215-1', NULL, DATEADD(day, -10, GETDATE()), DATEADD(day, -10, GETDATE())),
(2, 5280,  1, 2, 'TXN-20260217-2', NULL, DATEADD(day, -8,  GETDATE()), DATEADD(day, -8,  GETDATE())),
(3, 89990, 1, 1, 'TXN-20260220-3', NULL, DATEADD(day, -5,  GETDATE()), DATEADD(day, -5,  GETDATE())),
(4, 8990,  1, 3, 'TXN-20260222-4', NULL, DATEADD(day, -3,  GETDATE()), DATEADD(day, -3,  GETDATE()));

PRINT '  ✓ PAYMENTS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 12. ОТЗЫВЫ (REVIEWS) - camelCase
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ REVIEWS...';

INSERT INTO REVIEWS (userId, productId, orderId, rating, title, comment, isVerifiedPurchase, isApproved, helpfulCount, createdAt) VALUES
(1, 1, 1, 5, 'Отличный телефон!',      'Работает идеально, камера супер',     1, 1, 25, DATEADD(day, -8, GETDATE())),
(2, 4, 2, 4, 'Хорошая футболка',      'Качество отличное, размер подходит', 1, 1, 12, DATEADD(day, -6, GETDATE())),
(3, 1, 3, 5, 'Рекомендую Samsung',    'Быстрая доставка, всё работает',     1, 1, 18, DATEADD(day, -3, GETDATE())),
(4, 8, 4, 5, 'Отличные наушники',     'Звук чистый, долго держат заряд',    1, 1,  8, DATEADD(day, -1, GETDATE()));

PRINT '  ✓ REVIEWS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO


-- ============================================================
-- 13. ИЗБРАННОЕ (FAVORITES) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ FAVORITES...';

INSERT INTO FAVORITES (userid, productid, createdat) VALUES
(1, 1, DATEADD(day, -20, GETDATE())),
(1, 2, DATEADD(day, -15, GETDATE())),
(2, 4, DATEADD(day, -10, GETDATE())),
(3, 1, DATEADD(day, -5,  GETDATE())),
(4, 8, DATEADD(day, -3,  GETDATE()));

PRINT '  ✓ FAVORITES: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 14. АКЦИИ (PROMOTIONS) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ PROMOTIONS...';

INSERT INTO PROMOTIONS (name, description, discounttype, discountvalue, startdate, enddate, isactive, createdat) VALUES
('Зимняя распродажа',     'Скидки до 20% на электронику',           'percent', 20, DATEADD(day, -10, GETDATE()), DATEADD(day, 10, GETDATE()), 1, GETDATE()),
('Скидка на одежду',      'До 15% на всю одежду',                   'percent', 15, DATEADD(day, -5,  GETDATE()), DATEADD(day, 5,  GETDATE()), 1, GETDATE()),
('Скидка на книги',       '10% на все книги',                       'percent', 10, DATEADD(day, -3,  GETDATE()), DATEADD(day, 3,  GETDATE()), 1, GETDATE());

PRINT '  ✓ PROMOTIONS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 15. ТОВАРЫ НА АКЦИЯХ (PROMOTION_PRODUCTS) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ PROMOTION_PRODUCTS...';

INSERT INTO PROMOTIONPRODUCTS (promotionid, productid) VALUES
(1, 1), (1, 2), (1, 3),
(2, 4), (2, 5), (2, 6),
(3, 9);

PRINT '  ✓ PROMOTION_PRODUCTS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 16. ИСТОРИЯ ЦЕН (PRICE_HISTORY) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ PRICE_HISTORY...';

INSERT INTO PRICEHISTORY (skuid, oldprice, newprice, changedby, changedat) VALUES
(1, 95000, 89990, 16, DATEADD(day, -15, GETDATE())),
(2, 85000, 79990, 16, DATEADD(day, -12, GETDATE())),
(4, 2200,  1990,  16, DATEADD(day, -10, GETDATE())),
(6, 5500,  4990,  16, DATEADD(day, -8,  GETDATE()));

PRINT '  ✓ PRICE_HISTORY: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 17. ЛОГИ ПОЛЬЗОВАТЕЛЕЙ (USER_LOGS) - ВЫРОВНЕНЫ
-- ============================================================
PRINT 'ЗАПОЛНЕНИЕ ТАБЛИЦЫ USER_LOGS...';

INSERT INTO UserLogs (userId, action, entityType, entityId, details, ipAddress, createdAt) VALUES
(1, 'LOGIN',         NULL, NULL, '{"timestamp":"2026-02-20 10:30:00"}', '192.168.1.101', DATEADD(day, -5,  GETDATE())),
(1, 'VIEW_PRODUCT',  'PRODUCT', 1,    '{"productId":1}',              '192.168.1.101', DATEADD(day, -5,  GETDATE())),
(2, 'ADD_TO_CART',   'CART',    1,    '{"cartId":1}',                 '192.168.1.102', DATEADD(day, -4,  GETDATE())),
(3, 'CREATE_ORDER',  'ORDER',   3,    '{"orderId":3}',                '192.168.1.103', DATEADD(day, -3,  GETDATE())),
(4, 'VIEW_PRODUCT',  'PRODUCT', 8,    '{"productId":8}',              '192.168.1.104', DATEADD(day, -2,  GETDATE()));
PRINT '  ✓ USER_LOGS: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' записей';
GO

-- ============================================================
-- 18. ФИНАЛЬНАЯ СТАТИСТИКА - ВЫРОВНЕНА
-- ============================================================
PRINT '============================================================';
PRINT '✅ БАЗА ДАННЫХ ЗАПОЛНЕНА УСПЕШНО!';
PRINT '============================================================';

SELECT 
    'Таблица'           AS Таблица, COUNT(*) AS Записи FROM USERS UNION ALL
SELECT 'USERS',             COUNT(*)        FROM USERS UNION ALL
SELECT 'SELLERS',           COUNT(*)        FROM SELLERS UNION ALL
SELECT 'CATEGORIES',        COUNT(*)        FROM CATEGORIES UNION ALL
SELECT 'PRODUCTS',          COUNT(*)        FROM PRODUCTS UNION ALL
SELECT 'PRODUCT_SKU',       COUNT(*)        FROM ProductSkus UNION ALL
SELECT 'PRODUCT_IMAGES',    COUNT(*)        FROM PRODUCTIMAGES UNION ALL
SELECT 'CARTS',             COUNT(*)        FROM CARTS UNION ALL
SELECT 'CART_ITEMS',        COUNT(*)        FROM CARTITEMS UNION ALL
SELECT 'ORDERS',            COUNT(*)        FROM ORDERS UNION ALL
SELECT 'ORDER_ITEMS',       COUNT(*)        FROM ORDERITEMS UNION ALL
SELECT 'PAYMENTS',          COUNT(*)        FROM PAYMENTS UNION ALL
SELECT 'REVIEWS',           COUNT(*)        FROM REVIEWS UNION ALL
SELECT 'FAVORITES',         COUNT(*)        FROM FAVORITES UNION ALL
SELECT 'PROMOTIONS',        COUNT(*)        FROM PROMOTIONS UNION ALL
SELECT 'PROMOTION_PRODUCTS',COUNT(*)        FROM PROMOTIONPRODUCTS UNION ALL
SELECT 'PRICE_HISTORY',     COUNT(*)        FROM PRICEHISTORY UNION ALL
SELECT 'USER_LOGS',         COUNT(*)        FROM USERLOGS
ORDER BY Таблица;

PRINT '============================================================';
PRINT '🎉 СКРИПТ ВЫПОЛНЕН! Тестируйте приложение!';
PRINT '============================================================';
PRINT '👤 Seller ID: 1 (ТехноМир)';
PRINT '👤 Buyer ID: 1 (Иванов Иван)';
PRINT '============================================================';
GO
