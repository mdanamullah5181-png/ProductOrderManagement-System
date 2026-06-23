USE master
CREATE DATABASE MobileShopManagementDB
GO

USE MobileShopManagementDB
GO

-- =========================
-- Product Category Table
-- =========================
CREATE TABLE ProductCategory (
    ProductCategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    CategoryDescription NVARCHAR(500) NULL
)
GO

-- =========================
-- Product Table
-- =========================
CREATE TABLE Product (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(200) NOT NULL,
    Unit NVARCHAR(50) NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    AvailableQuantity INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    ProductImage NVARCHAR(500) NULL,
    ProductCategoryId INT FOREIGN KEY REFERENCES ProductCategory(ProductCategoryId)
)
GO

-- =========================
-- Customer Table
-- =========================
CREATE TABLE Customer (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    CustomerName NVARCHAR(200) NOT NULL,
    ContactNumber NVARCHAR(20) NOT NULL,
    ContactAddress NVARCHAR(500) NOT NULL
)
GO

-- =========================
-- Orders Table
-- =========================
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT FOREIGN KEY REFERENCES Customer(CustomerId),
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL
)
GO

-- =========================
-- Order Details Table
-- =========================
CREATE TABLE OrderDetails (
    OrderDetailsId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT FOREIGN KEY REFERENCES Orders(OrderId) ON DELETE CASCADE,
    ProductCategoryId INT FOREIGN KEY REFERENCES ProductCategory(ProductCategoryId),
    ProductId INT FOREIGN KEY REFERENCES Product(ProductId),
    OrderQuantity INT NOT NULL,
    OrderUnit NVARCHAR(50) NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL
)
GO

-- =============================================
-- Insert Sample Product Categories
-- =============================================
INSERT INTO ProductCategory (CategoryName, CategoryDescription) VALUES 
('Mobile Phones', 'Smartphones and feature phones'),
('Mobile Accessories', 'Chargers, cables, headphones, covers'),
('SIM & Network', 'SIM cards and network services'),
('Repair Services', 'Mobile servicing and repair'),
('Smart Devices', 'Smart watches and gadgets')
GO

-- =============================================
-- Insert Sample Products
-- =============================================
INSERT INTO Product (ProductName, Unit, UnitPrice, AvailableQuantity, ProductImage, ProductCategoryId) VALUES 
-- Mobile Phones
('Samsung Galaxy A15', 'Pcs', 18500.00, 40, NULL, 1),
('Xiaomi Redmi 13', 'Pcs', 16500.00, 50, NULL, 1),
('iPhone 14', 'Pcs', 95000.00, 15, NULL, 1),

-- Accessories
('Fast Charger', 'Pcs', 1200.00, 100, NULL, 2),
('USB Type-C Cable', 'Pcs', 350.00, 200, NULL, 2),
('Bluetooth Headset', 'Pcs', 1800.00, 60, NULL, 2),

-- SIM & Network
('GP SIM', 'Pcs', 200.00, 300, NULL, 3),
('Robi SIM', 'Pcs', 200.00, 250, NULL, 3),

-- Repair Services
('Screen Replacement', 'Job', 2500.00, 9999, NULL, 4),
('Battery Change', 'Job', 1200.00, 9999, NULL, 4),

-- Smart Devices
('Smart Watch', 'Pcs', 3500.00, 45, NULL, 5)
GO

-- =============================================
-- Insert Sample Customers
-- =============================================
INSERT INTO Customer (CustomerName, ContactNumber, ContactAddress) VALUES 
('Md. Karim', '01811111111', 'Dhaka'),
('Hasan Ali', '01822222222', 'Chattogram'),
('Nusrat Jahan', '01833333333', 'Cumilla')
GO

-- =============================================
-- Insert Sample Orders
-- =============================================
DECLARE @Customer1 INT = (SELECT CustomerId FROM Customer WHERE ContactNumber='01811111111')
DECLARE @Customer2 INT = (SELECT CustomerId FROM Customer WHERE ContactNumber='01822222222')

-- Order 1
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount)
VALUES (@Customer1, GETDATE()-2, 19700.00)

DECLARE @Order1 INT = SCOPE_IDENTITY()

INSERT INTO OrderDetails 
(OrderId, ProductCategoryId, ProductId, OrderQuantity, OrderUnit, UnitPrice, Amount)
VALUES
(@Order1, 1, 1, 1, 'Pcs', 18500.00, 18500.00),
(@Order1, 2, 4, 1, 'Pcs', 1200.00, 1200.00)

-- Order 2
INSERT INTO Orders (CustomerId, OrderDate, TotalAmount)
VALUES (@Customer2, GETDATE()-1, 350.00)

DECLARE @Order2 INT = SCOPE_IDENTITY()

INSERT INTO OrderDetails
(OrderId, ProductCategoryId, ProductId, OrderQuantity, OrderUnit, UnitPrice, Amount)
VALUES
(@Order2, 2, 5, 1, 'Pcs', 350.00, 350.00)
GO

 
SELECT * FROM Customer

SELECT * FROM ProductCategory

SELECT * FROM Product

SELECT * FROM Orders

SELECT * FROM OrderDetails
