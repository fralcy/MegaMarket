-- =============================================
-- MegaMarket Database Setup Script
-- Generated from EF Core Models and Migrations
-- SQL Server Database
-- =============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'MegaMarketDb')
BEGIN
    ALTER DATABASE MegaMarketDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MegaMarketDb;
END
GO

-- Create database
CREATE DATABASE MegaMarketDb;
GO

USE MegaMarketDb;
GO

-- =============================================
-- CREATE TABLES
-- =============================================

-- Users Table
CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(100) NOT NULL,
    username NVARCHAR(50) NOT NULL,
    password NVARCHAR(255) NOT NULL,
    role NVARCHAR(20) NOT NULL DEFAULT 'Employee',
    phone NVARCHAR(15) NULL,
    email NVARCHAR(100) NULL,
    CONSTRAINT UQ_Users_Username UNIQUE (username)
);

-- ShiftTypes Table
CREATE TABLE ShiftTypes (
    shift_type_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    wage_per_hour DECIMAL(10,2) NOT NULL
);

-- Attendances Table
CREATE TABLE Attendances (
    attendance_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    shift_type_id INT NOT NULL,
    date DATETIME2 NOT NULL,
    check_in DATETIME2 NULL,
    check_out DATETIME2 NULL,
    is_late BIT NOT NULL DEFAULT 0,
    note NVARCHAR(MAX) NULL,
    CONSTRAINT FK_Attendances_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id) ON DELETE CASCADE,
    CONSTRAINT FK_Attendances_ShiftTypes FOREIGN KEY (shift_type_id)
        REFERENCES ShiftTypes(shift_type_id) ON DELETE NO ACTION
);

-- Suppliers Table
CREATE TABLE Suppliers (
    supplier_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    address NVARCHAR(255) NULL,
    email NVARCHAR(100) NULL,
    phone NVARCHAR(15) NULL
);

-- Products Table
CREATE TABLE Products (
    product_id INT IDENTITY(1,1) PRIMARY KEY,
    barcode NVARCHAR(100) NOT NULL,
    name NVARCHAR(200) NOT NULL,
    category NVARCHAR(100) NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    quantity_in_stock INT NOT NULL DEFAULT 0,
    min_quantity INT NOT NULL DEFAULT 0,
    expiry_date DATETIME2 NULL,
    is_perishable BIT NOT NULL DEFAULT 0,
    image_url NVARCHAR(500) NULL,
    unit_label NVARCHAR(50) NULL,
    original_price DECIMAL(10,2) NULL,
    discount_percent INT NULL,
    CONSTRAINT UQ_Products_Barcode UNIQUE (barcode)
);

-- Imports Table
CREATE TABLE Imports (
    import_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    supplier_id INT NOT NULL,
    import_date DATETIME2 NOT NULL,
    total_cost DECIMAL(12,2) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_Imports_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id) ON DELETE NO ACTION,
    CONSTRAINT FK_Imports_Suppliers FOREIGN KEY (supplier_id)
        REFERENCES Suppliers(supplier_id) ON DELETE NO ACTION
);

-- ImportDetails Table
CREATE TABLE ImportDetails (
    import_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    expiry_date DATETIME2 NULL,
    PRIMARY KEY (import_id, product_id),
    CONSTRAINT FK_ImportDetails_Imports FOREIGN KEY (import_id)
        REFERENCES Imports(import_id) ON DELETE CASCADE,
    CONSTRAINT FK_ImportDetails_Products FOREIGN KEY (product_id)
        REFERENCES Products(product_id) ON DELETE NO ACTION
);

-- Customers Table
CREATE TABLE Customers (
    customer_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name NVARCHAR(100) NOT NULL,
    phone NVARCHAR(15) NULL,
    email NVARCHAR(100) NULL,
    points INT NOT NULL DEFAULT 0,
    rank NVARCHAR(20) NOT NULL DEFAULT 'Silver'
);

-- Promotions Table
CREATE TABLE Promotions (
    promotion_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    description NVARCHAR(MAX) NULL,
    discount_type NVARCHAR(20) NOT NULL DEFAULT 'percent',
    discount_value DECIMAL(10,2) NOT NULL,
    start_date DATETIME2 NOT NULL,
    end_date DATETIME2 NOT NULL,
    type NVARCHAR(50) NOT NULL DEFAULT 'invoice'
);

-- PromotionProducts Table (Many-to-Many)
CREATE TABLE PromotionProducts (
    promotion_id INT NOT NULL,
    product_id INT NOT NULL,
    PRIMARY KEY (promotion_id, product_id),
    CONSTRAINT FK_PromotionProducts_Promotions FOREIGN KEY (promotion_id)
        REFERENCES Promotions(promotion_id) ON DELETE CASCADE,
    CONSTRAINT FK_PromotionProducts_Products FOREIGN KEY (product_id)
        REFERENCES Products(product_id) ON DELETE CASCADE
);

-- Invoices Table
CREATE TABLE Invoices (
    invoice_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    customer_id INT NULL,
    created_at DATETIME2 NOT NULL,
    total_before_discount DECIMAL(12,2) NOT NULL,
    total_amount DECIMAL(12,2) NOT NULL,
    payment_method NVARCHAR(50) NOT NULL DEFAULT 'cash',
    received_amount DECIMAL(12,2) NOT NULL,
    change_amount DECIMAL(12,2) NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    promotion_id INT NULL,
    CONSTRAINT FK_Invoices_Users FOREIGN KEY (user_id)
        REFERENCES Users(user_id) ON DELETE NO ACTION,
    CONSTRAINT FK_Invoices_Customers FOREIGN KEY (customer_id)
        REFERENCES Customers(customer_id) ON DELETE SET NULL,
    CONSTRAINT FK_Invoices_Promotions FOREIGN KEY (promotion_id)
        REFERENCES Promotions(promotion_id) ON DELETE SET NULL
);

-- InvoiceDetails Table
CREATE TABLE InvoiceDetails (
    invoice_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    discount_per_unit DECIMAL(10,2) NOT NULL DEFAULT 0,
    promotion_id INT NULL,
    PRIMARY KEY (invoice_id, product_id),
    CONSTRAINT FK_InvoiceDetails_Invoices FOREIGN KEY (invoice_id)
        REFERENCES Invoices(invoice_id) ON DELETE CASCADE,
    CONSTRAINT FK_InvoiceDetails_Products FOREIGN KEY (product_id)
        REFERENCES Products(product_id) ON DELETE NO ACTION,
    CONSTRAINT FK_InvoiceDetails_Promotions FOREIGN KEY (promotion_id)
        REFERENCES Promotions(promotion_id) ON DELETE SET NULL
);

-- Rewards Table
CREATE TABLE Rewards (
    reward_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    description NVARCHAR(MAX) NULL,
    point_cost INT NOT NULL,
    reward_type NVARCHAR(20) NOT NULL DEFAULT 'Gift',
    value DECIMAL(10,2) NULL,
    quantity_available INT NOT NULL,
    is_active BIT NOT NULL DEFAULT 1
);

-- CustomerRewards Table
CREATE TABLE CustomerRewards (
    redemption_id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT NOT NULL,
    reward_id INT NOT NULL,
    invoice_id INT NULL,
    redeemed_at DATETIME2 NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    used_at DATETIME2 NULL,
    CONSTRAINT FK_CustomerRewards_Customers FOREIGN KEY (customer_id)
        REFERENCES Customers(customer_id) ON DELETE CASCADE,
    CONSTRAINT FK_CustomerRewards_Rewards FOREIGN KEY (reward_id)
        REFERENCES Rewards(reward_id) ON DELETE NO ACTION,
    CONSTRAINT FK_CustomerRewards_Invoices FOREIGN KEY (invoice_id)
        REFERENCES Invoices(invoice_id) ON DELETE SET NULL
);

-- PointTransactions Table
CREATE TABLE PointTransactions (
    transaction_id INT IDENTITY(1,1) PRIMARY KEY,
    invoice_id INT NULL,
    customer_id INT NOT NULL,
    point_change INT NOT NULL,
    transaction_type NVARCHAR(50) NOT NULL DEFAULT 'Earn',
    created_at DATETIME2 NOT NULL,
    description NVARCHAR(MAX) NULL,
    CONSTRAINT FK_PointTransactions_Invoices FOREIGN KEY (invoice_id)
        REFERENCES Invoices(invoice_id) ON DELETE SET NULL,
    CONSTRAINT FK_PointTransactions_Customers FOREIGN KEY (customer_id)
        REFERENCES Customers(customer_id) ON DELETE CASCADE
);

-- OrderRequests Table
CREATE TABLE OrderRequests (
    order_id INT IDENTITY(1,1) PRIMARY KEY,
    product_id INT NOT NULL,
    requested_quantity INT NOT NULL,
    request_date DATETIME2 NOT NULL,
    status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_OrderRequests_Products FOREIGN KEY (product_id)
        REFERENCES Products(product_id) ON DELETE CASCADE
);

GO

-- =============================================
-- CREATE INDEXES
-- =============================================

-- Users Indexes
CREATE UNIQUE INDEX IX_Users_Username ON Users(username);

-- Attendances Indexes
CREATE INDEX IX_Attendances_ShiftTypeId ON Attendances(shift_type_id);
CREATE INDEX IX_Attendances_UserId_Date ON Attendances(user_id, date);

-- Products Indexes
CREATE UNIQUE INDEX IX_Products_Barcode ON Products(barcode);
CREATE INDEX IX_Products_Category ON Products(category);
CREATE INDEX IX_Products_IsPerishable_ExpiryDate ON Products(is_perishable, expiry_date);
CREATE INDEX IX_Products_QuantityInStock_MinQuantity ON Products(quantity_in_stock, min_quantity);

-- Imports Indexes
CREATE INDEX IX_Imports_ImportDate ON Imports(import_date);
CREATE INDEX IX_Imports_SupplierId ON Imports(supplier_id);
CREATE INDEX IX_Imports_UserId ON Imports(user_id);

-- ImportDetails Indexes
CREATE INDEX IX_ImportDetails_ProductId ON ImportDetails(product_id);

-- Customers Indexes
CREATE INDEX IX_Customers_Phone ON Customers(phone);

-- Invoices Indexes
CREATE INDEX IX_Invoices_CreatedAt ON Invoices(created_at);
CREATE INDEX IX_Invoices_CustomerId ON Invoices(customer_id);
CREATE INDEX IX_Invoices_PromotionId ON Invoices(promotion_id);
CREATE INDEX IX_Invoices_Status ON Invoices(status);
CREATE INDEX IX_Invoices_UserId ON Invoices(user_id);

-- InvoiceDetails Indexes
CREATE INDEX IX_InvoiceDetails_ProductId ON InvoiceDetails(product_id);
CREATE INDEX IX_InvoiceDetails_PromotionId ON InvoiceDetails(promotion_id);

-- CustomerRewards Indexes
CREATE INDEX IX_CustomerRewards_CustomerId ON CustomerRewards(customer_id);
CREATE INDEX IX_CustomerRewards_InvoiceId ON CustomerRewards(invoice_id);
CREATE INDEX IX_CustomerRewards_RewardId ON CustomerRewards(reward_id);

-- PointTransactions Indexes
CREATE INDEX IX_PointTransactions_InvoiceId ON PointTransactions(invoice_id);
CREATE INDEX IX_PointTransactions_CustomerId_CreatedAt_TransactionType
    ON PointTransactions(customer_id, created_at, transaction_type);

-- OrderRequests Indexes
CREATE INDEX IX_OrderRequests_ProductId ON OrderRequests(product_id);

-- PromotionProducts Indexes
CREATE INDEX IX_PromotionProducts_ProductId ON PromotionProducts(product_id);

GO

-- =============================================
-- SEED DATA
-- =============================================

PRINT 'Seeding Users...';

-- Insert Users (4 roles: Admin, Manager, Cashier, Warehouse)
-- Note: Passwords are BCrypt hashed for "Password123!"
INSERT INTO Users (full_name, username, password, role, email, phone) VALUES
('Admin User', 'admin', '$2a$11$bMKJZPzQXqZqvvJ4LZxnWO8qK2zYJC9vLG6FxPQZWJYrYhXLZXNLW', 'Admin', 'admin@megamarket.com', '0901234567'),
('John Manager', 'john.manager', '$2a$11$bMKJZPzQXqZqvvJ4LZxnWO8qK2zYJC9vLG6FxPQZWJYrYhXLZXNLW', 'Manager', 'john@megamarket.com', '0907654321'),
('Sarah Cashier', 'sarah.cashier', '$2a$11$bMKJZPzQXqZqvvJ4LZxnWO8qK2zYJC9vLG6FxPQZWJYrYhXLZXNLW', 'Cashier', 'sarah@megamarket.com', '0912345678'),
('Mike Warehouse', 'mike.warehouse', '$2a$11$bMKJZPzQXqZqvvJ4LZxnWO8qK2zYJC9vLG6FxPQZWJYrYhXLZXNLW', 'Warehouse', 'mike@megamarket.com', '0923456789');

PRINT 'Seeding ShiftTypes...';

-- Insert ShiftTypes
INSERT INTO ShiftTypes (name, start_time, end_time, wage_per_hour) VALUES
('Morning Shift', '06:00:00', '14:00:00', 40000),
('Afternoon Shift', '14:00:00', '22:00:00', 45000),
('Night Shift', '22:00:00', '06:00:00', 55000),
('Full Day', '08:00:00', '17:00:00', 42000);

PRINT 'Seeding Attendances...';

-- Insert Attendances (sample data for last 7 days)
DECLARE @AdminUserId INT = (SELECT user_id FROM Users WHERE username = 'admin');
DECLARE @ManagerUserId INT = (SELECT user_id FROM Users WHERE username = 'john.manager');
DECLARE @CashierUserId INT = (SELECT user_id FROM Users WHERE username = 'sarah.cashier');
DECLARE @WarehouseUserId INT = (SELECT user_id FROM Users WHERE username = 'mike.warehouse');

DECLARE @MorningShiftId INT = (SELECT shift_type_id FROM ShiftTypes WHERE name = 'Morning Shift');
DECLARE @AfternoonShiftId INT = (SELECT shift_type_id FROM ShiftTypes WHERE name = 'Afternoon Shift');
DECLARE @FullDayShiftId INT = (SELECT shift_type_id FROM ShiftTypes WHERE name = 'Full Day');

-- Admin attendances (last 7 days)
INSERT INTO Attendances (user_id, shift_type_id, date, check_in, check_out, is_late, note)
SELECT
    @AdminUserId,
    @FullDayShiftId,
    CAST(DATEADD(DAY, -number, GETDATE()) AS DATE),
    DATEADD(MINUTE, CASE WHEN number % 2 = 0 THEN 0 ELSE 5 END, DATEADD(HOUR, 8, DATEADD(DAY, -number, GETDATE()))),
    DATEADD(MINUTE, CASE WHEN number % 2 = 0 THEN 0 ELSE 10 END, DATEADD(HOUR, 17, DATEADD(DAY, -number, GETDATE()))),
    CASE WHEN number % 2 = 0 THEN 0 ELSE 1 END,
    CASE WHEN number % 2 = 0 THEN 'On time' ELSE 'Late check-in' END
FROM (SELECT 1 AS number UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5 UNION SELECT 6 UNION SELECT 7) AS Numbers;

-- Manager attendances (last 5 days)
INSERT INTO Attendances (user_id, shift_type_id, date, check_in, check_out, is_late, note)
SELECT
    @ManagerUserId,
    @MorningShiftId,
    CAST(DATEADD(DAY, -number, GETDATE()) AS DATE),
    DATEADD(HOUR, 6, DATEADD(DAY, -number, GETDATE())),
    DATEADD(HOUR, 14, DATEADD(DAY, -number, GETDATE())),
    0,
    'Regular shift'
FROM (SELECT 1 AS number UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) AS Numbers;

-- Cashier attendances (last 3 days)
INSERT INTO Attendances (user_id, shift_type_id, date, check_in, check_out, is_late, note)
SELECT
    @CashierUserId,
    @AfternoonShiftId,
    CAST(DATEADD(DAY, -number, GETDATE()) AS DATE),
    DATEADD(MINUTE, CASE WHEN number % 2 = 0 THEN 0 ELSE 5 END, DATEADD(HOUR, 14, DATEADD(DAY, -number, GETDATE()))),
    DATEADD(HOUR, 22, DATEADD(DAY, -number, GETDATE())),
    CASE WHEN number % 2 = 0 THEN 0 ELSE 1 END,
    CASE WHEN number % 2 = 0 THEN 'On time' ELSE 'Traffic delay' END
FROM (SELECT 1 AS number UNION SELECT 2 UNION SELECT 3) AS Numbers;

-- Warehouse attendances (last 4 days)
INSERT INTO Attendances (user_id, shift_type_id, date, check_in, check_out, is_late, note)
SELECT
    @WarehouseUserId,
    @FullDayShiftId,
    CAST(DATEADD(DAY, -number, GETDATE()) AS DATE),
    DATEADD(HOUR, 8, DATEADD(DAY, -number, GETDATE())),
    DATEADD(HOUR, 17, DATEADD(DAY, -number, GETDATE())),
    0,
    'Inventory check'
FROM (SELECT 1 AS number UNION SELECT 2 UNION SELECT 3 UNION SELECT 4) AS Numbers;

PRINT 'Seeding Suppliers...';

-- Insert Suppliers
INSERT INTO Suppliers (name, phone) VALUES
('Fresh Foods Co.', '555-1200'),
('Dairy Direct', '555-8831'),
('Global Groceries', '555-7412'),
('Premium Produce', '555-9102');

PRINT 'Seeding Products...';

-- Insert Products
INSERT INTO Products (barcode, name, category, unit_label, unit_price, quantity_in_stock, min_quantity, is_perishable, expiry_date) VALUES
('1110001110001', 'Organic Apples 1kg', 'Produce', 'Bag', 45000, 120, 30, 1, DATEADD(DAY, 10, CAST(GETDATE() AS DATE))),
('2220002220002', 'Whole Milk 1L', 'Dairy', 'Bottle', 18000, 220, 60, 1, DATEADD(DAY, 5, CAST(GETDATE() AS DATE))),
('3330003330003', 'Free Range Eggs 12ct', 'Dairy', 'Carton', 52000, 150, 40, 1, DATEADD(DAY, 2, CAST(GETDATE() AS DATE))),
('4440004440004', 'Roasted Coffee Beans 500g', 'Beverages', 'Bag', 95000, 80, 20, 0, NULL),
('5550005550005', 'Bottled Water 24x500ml', 'Beverages', 'Pack', 120000, 60, 15, 0, NULL);

PRINT 'Seeding Imports...';

-- Insert Imports
DECLARE @FreshFoodsId INT = (SELECT supplier_id FROM Suppliers WHERE name = 'Fresh Foods Co.');
DECLARE @DairyDirectId INT = (SELECT supplier_id FROM Suppliers WHERE name = 'Dairy Direct');
DECLARE @GlobalGroceriesId INT = (SELECT supplier_id FROM Suppliers WHERE name = 'Global Groceries');
DECLARE @PremiumProduceId INT = (SELECT supplier_id FROM Suppliers WHERE name = 'Premium Produce');

DECLARE @ApplesId INT = (SELECT product_id FROM Products WHERE barcode = '1110001110001');
DECLARE @MilkId INT = (SELECT product_id FROM Products WHERE barcode = '2220002220002');
DECLARE @EggsId INT = (SELECT product_id FROM Products WHERE barcode = '3330003330003');
DECLARE @CoffeeId INT = (SELECT product_id FROM Products WHERE barcode = '4440004440004');
DECLARE @WaterId INT = (SELECT product_id FROM Products WHERE barcode = '5550005550005');

-- Import 1: Fresh Foods Co. (Completed)
INSERT INTO Imports (user_id, supplier_id, import_date, total_cost, status)
VALUES (@AdminUserId, @FreshFoodsId, DATEADD(DAY, -8, CAST(GETDATE() AS DATE)), 6360000, 'Completed');

DECLARE @Import1Id INT = SCOPE_IDENTITY();

INSERT INTO ImportDetails (import_id, product_id, quantity, unit_price, expiry_date) VALUES
(@Import1Id, @ApplesId, 80, 42000, DATEADD(DAY, 12, CAST(GETDATE() AS DATE))),
(@Import1Id, @EggsId, 60, 50000, DATEADD(DAY, 5, CAST(GETDATE() AS DATE)));

-- Import 2: Dairy Direct (Completed)
INSERT INTO Imports (user_id, supplier_id, import_date, total_cost, status)
VALUES (@AdminUserId, @DairyDirectId, DATEADD(DAY, -3, CAST(GETDATE() AS DATE)), 2040000, 'Completed');

DECLARE @Import2Id INT = SCOPE_IDENTITY();

INSERT INTO ImportDetails (import_id, product_id, quantity, unit_price, expiry_date) VALUES
(@Import2Id, @MilkId, 120, 17000, DATEADD(DAY, 6, CAST(GETDATE() AS DATE)));

-- Import 3: Global Groceries (Draft)
INSERT INTO Imports (user_id, supplier_id, import_date, total_cost, status)
VALUES (@AdminUserId, @GlobalGroceriesId, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)), 9100000, 'Draft');

DECLARE @Import3Id INT = SCOPE_IDENTITY();

INSERT INTO ImportDetails (import_id, product_id, quantity, unit_price, expiry_date) VALUES
(@Import3Id, @CoffeeId, 50, 90000, NULL),
(@Import3Id, @WaterId, 40, 115000, NULL);

-- Import 4: Premium Produce (Cancelled)
INSERT INTO Imports (user_id, supplier_id, import_date, total_cost, status)
VALUES (@AdminUserId, @PremiumProduceId, DATEADD(DAY, -15, CAST(GETDATE() AS DATE)), 1720000, 'Cancelled');

DECLARE @Import4Id INT = SCOPE_IDENTITY();

INSERT INTO ImportDetails (import_id, product_id, quantity, unit_price, expiry_date) VALUES
(@Import4Id, @ApplesId, 40, 43000, DATEADD(DAY, -1, CAST(GETDATE() AS DATE)));

PRINT 'Database setup and seeding completed successfully!';
PRINT '';
PRINT 'Login credentials (password: Password123! for all):';
PRINT '  Admin:     username = admin';
PRINT '  Manager:   username = john.manager';
PRINT '  Cashier:   username = sarah.cashier';
PRINT '  Warehouse: username = mike.warehouse';

GO
