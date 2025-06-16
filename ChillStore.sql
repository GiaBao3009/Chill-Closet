-- KỊCH BẢN TẠO DATABASE CHO DỰ ÁN CHILL CLOSET
-- Tên Database: ChillStore

Create Database ChillStore;

-- 1. Bảng Roles (Phân Quyền)
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- 2. Bảng Users (Người Dùng)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(150),
    Email VARCHAR(255) NOT NULL UNIQUE,
    PhoneNumber VARCHAR(20),
    Address NVARCHAR(500),
    PasswordHash NVARCHAR(MAX) NOT NULL,
    RoleId INT NOT NULL,
    DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
GO

-- 3. Bảng Categories (Danh mục sản phẩm)
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    ParentCategoryId INT NULL,
    CONSTRAINT FK_Categories_Self FOREIGN KEY (ParentCategoryId) REFERENCES Categories(Id)
);
GO

-- 4. Bảng Brands (Thương hiệu)
CREATE TABLE Brands (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);
GO

-- 5. Bảng Tags
CREATE TABLE Tags (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- 6. Bảng Products (Sản phẩm)
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    MainImageUrl VARCHAR(500) NOT NULL,
    IsNewArrival BIT NOT NULL DEFAULT 0,
    IsHotSale BIT NOT NULL DEFAULT 0,
    IsBestSeller BIT NOT NULL DEFAULT 0,
    CategoryId INT NOT NULL,
    BrandId INT,
    DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    CONSTRAINT FK_Products_Brands FOREIGN KEY (BrandId) REFERENCES Brands(Id)
);
GO

-- 7. Bảng ProductVariants (Các biến thể của sản phẩm: size, màu sắc)
CREATE TABLE ProductVariants (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT NOT NULL,
    Size NVARCHAR(50),
    Color NVARCHAR(50),
    Price DECIMAL(18, 2) NOT NULL,
    SalePrice DECIMAL(18, 2),
    StockQuantity INT NOT NULL DEFAULT 0,
    SKU VARCHAR(100) UNIQUE,
    CONSTRAINT FK_ProductVariants_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);
GO

-- 8. Bảng ProductImages (Các ảnh khác của sản phẩm)
CREATE TABLE ProductImages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT NOT NULL,
    ImageUrl VARCHAR(500) NOT NULL,
    CONSTRAINT FK_ProductImages_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);
GO

-- 9. Bảng ProductTags (Bảng nối quan hệ nhiều-nhiều giữa Products và Tags)
CREATE TABLE ProductTags (
    ProductId INT NOT NULL,
    TagId INT NOT NULL,
    CONSTRAINT PK_ProductTags PRIMARY KEY (ProductId, TagId),
    CONSTRAINT FK_ProductTags_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ProductTags_Tags FOREIGN KEY (TagId) REFERENCES Tags(Id) ON DELETE CASCADE
);
GO

-- 10. Bảng Reviews (Đánh giá sản phẩm)
CREATE TABLE Reviews (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT NOT NULL,
    UserId INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(1000),
    ReviewDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- 11. Bảng Orders (Đơn hàng)
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT,
    OrderDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    TotalPrice DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CustomerName NVARCHAR(150) NOT NULL,
    ShippingAddress NVARCHAR(500) NOT NULL,
    ShippingPhone VARCHAR(20) NOT NULL,
    ShippingEmail VARCHAR(255) NOT NULL,
    OrderNotes NVARCHAR(1000),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- 12. Bảng OrderItems (Chi tiết đơn hàng)
CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductVariantId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_ProductVariants FOREIGN KEY (ProductVariantId) REFERENCES ProductVariants(Id)
);
GO

-- 13. Bảng Coupons (Mã giảm giá)
CREATE TABLE Coupons (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code VARCHAR(50) NOT NULL UNIQUE,
    DiscountAmount DECIMAL(18, 2),
    DiscountPercent INT,
    ExpiryDate DATETIME2,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- 14. Bảng BlogPosts (Bài viết Blog)
CREATE TABLE BlogPosts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(500) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    ImageUrl VARCHAR(500),
    PublishedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    AuthorId INT NOT NULL,
    IsPublished BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_BlogPosts_Users FOREIGN KEY (AuthorId) REFERENCES Users(Id)
);
GO


-- KỊCH BẢN INSERT DỮ LIỆU MẪU CHO DATABASE CHILLSTORE

-- 1. Thêm dữ liệu cho bảng Roles
INSERT INTO Roles (Name) VALUES
('Admin'),
('Customer');
GO

-- 2. Thêm dữ liệu cho bảng Categories
INSERT INTO Categories (Name) VALUES
('Men'),
('Women'),
('Bags'),
('Clothing'),
('Shoes'),
('Accessories'),
('Kids');
GO

-- 3. Thêm dữ liệu cho bảng Brands
INSERT INTO Brands (Name) VALUES
('Louis Vuitton'),
('Chanel'),
('Hermes'),
('Gucci');
GO

-- 4. Thêm dữ liệu cho bảng Tags
INSERT INTO Tags (Name) VALUES
('Product'),
('Bags'),
('Shoes'),
('Fashion'),
('Clothing'),
('Hats'),
('Accessories');
GO

-- 5. Thêm dữ liệu cho bảng Products (Sản phẩm mẫu)
-- Giả sử CategoryId=4 (Clothing), BrandId=4 (Gucci)
INSERT INTO Products (Name, Description, MainImageUrl, IsNewArrival, CategoryId, BrandId) VALUES
('Piqué Biker Jacket', 'A cool biker jacket for men.', '/img/product/product-1.jpg', 1, 4, 4),
('Multi-pocket Chest Bag', 'A stylish multi-pocket chest bag.', '/img/product/product-3.jpg', 1, 3, 2),
('Ankle Boots', 'Comfortable and stylish ankle boots.', '/img/product/product-6.jpg', 0, 5, 3);
GO

-- 6. Thêm dữ liệu cho bảng ProductVariants (Biến thể sản phẩm mẫu)
-- Giả sử ProductId=1 là 'Piqué Biker Jacket'
INSERT INTO ProductVariants (ProductId, Size, Color, Price, SalePrice, StockQuantity, SKU) VALUES
(1, 'M', 'Black', 67.24, NULL, 50, 'SKU001-M-BLK'),
(1, 'L', 'Black', 67.24, NULL, 30, 'SKU001-L-BLK'),
(1, 'M', 'Grey', 67.24, NULL, 40, 'SKU001-M-GRY');
-- Giả sử ProductId=2 là 'Multi-pocket Chest Bag'
INSERT INTO ProductVariants (ProductId, Size, Color, Price, SalePrice, StockQuantity, SKU) VALUES
(2, NULL, 'Black', 50.00, 43.48, 100, 'SKU002-BLK');
-- Giả sử ProductId=3 là 'Ankle Boots'
INSERT INTO ProductVariants (ProductId, Size, Color, Price, SalePrice, StockQuantity, SKU) VALUES
(3, '39', 'Brown', 120.00, 98.49, 25, 'SKU003-39-BRN'),
(3, '40', 'Brown', 120.00, 98.49, 25, 'SKU003-40-BRN');
GO
