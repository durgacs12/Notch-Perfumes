-- Notch Perfumes Database Schema and Initial Seed Data
USE Notchperfumesdb;
GO

-- Create Categories Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        Id NVARCHAR(50) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX),
        Image NVARCHAR(MAX)
    );
END;
GO

-- Create Products Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id NVARCHAR(50) PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Subtitle NVARCHAR(250),
        Category NVARCHAR(50),
        ScentFamily NVARCHAR(50),
        Price DECIMAL(18, 2) NOT NULL,
        OriginalPrice DECIMAL(18, 2),
        Rating DECIMAL(3, 2) DEFAULT 5.0,
        ReviewsCount INT DEFAULT 0,
        Badge NVARCHAR(50),
        Image NVARCHAR(MAX),
        TopNotes NVARCHAR(MAX),
        HeartNotes NVARCHAR(MAX),
        BaseNotes NVARCHAR(MAX),
        Perfumer NVARCHAR(150),
        Description NVARCHAR(MAX),
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Create Orders Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
BEGIN
    CREATE TABLE Orders (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OrderNumber NVARCHAR(50) NOT NULL,
        CustomerName NVARCHAR(150) NOT NULL,
        CustomerEmail NVARCHAR(150),
        CustomerPhone NVARCHAR(50),
        ShippingAddress NVARCHAR(MAX),
        TotalAmount DECIMAL(18, 2) NOT NULL,
        PaymentMethod NVARCHAR(50) DEFAULT 'COD',
        Status NVARCHAR(50) DEFAULT 'Pending',
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Create OrderItems Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE OrderItems (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT FOREIGN KEY REFERENCES Orders(Id) ON DELETE CASCADE,
        ProductId NVARCHAR(50),
        ProductName NVARCHAR(150),
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18, 2) NOT NULL,
        TotalPrice DECIMAL(18, 2) NOT NULL
    );
END;
GO

-- Create Coupons Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Coupons')
BEGIN
    CREATE TABLE Coupons (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL UNIQUE,
        DiscountPercent DECIMAL(5, 2) NOT NULL,
        MinSpend DECIMAL(18, 2) DEFAULT 0,
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Create Users / Admin Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL UNIQUE,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        Password NVARCHAR(250) NOT NULL,
        Role NVARCHAR(50) DEFAULT 'Admin',
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Seed Categories Data
IF NOT EXISTS (SELECT * FROM Categories)
BEGIN
    INSERT INTO Categories (Id, Name, Description, Image) VALUES
    ('men', 'Men''s Fragrances', 'Bold, woody, and refreshing fragrances crafted for men.', 'https://images.unsplash.com/photo-1594035910387-fea47794261f'),
    ('women', 'Women''s Fragrances', 'Elegant floral, fruity, and amber perfumes for women.', 'https://images.unsplash.com/photo-1541643600914-78b084683601'),
    ('unisex', 'Unisex Fragrances', 'Mediterranean oceanic & versatile luxury scents.', 'https://images.unsplash.com/photo-1523293182086-7651a899d37f'),
    ('gifting', 'Gift Sets', 'Curated luxury perfume gift boxes & discovery sets.', 'https://images.unsplash.com/photo-1547887537-6158d64c35b3');
END;
GO

-- Seed Sample Products Data
IF NOT EXISTS (SELECT * FROM Products)
BEGIN
    INSERT INTO Products (Id, Name, Subtitle, Category, ScentFamily, Price, OriginalPrice, Rating, ReviewsCount, Badge, Image, TopNotes, HeartNotes, BaseNotes, Perfumer, Description) VALUES
    ('notch-raw-men', 'Notch Raw Eau De Parfum', 'For Men • Fresh & Citrus Woody', 'men', 'citrus', 119.00, 139.00, 4.8, 420, 'Bestseller', 'https://images.unsplash.com/photo-1594035910387-fea47794261f?auto=format&fit=crop&w=800&q=80', 'Bergamot, Lemon, Crisp Watery Accord', 'Violet Leaves, Geranium, Lavender', 'Guaiac Wood, Patchouli, Cashmeran', 'Olivier Pescheux (Qatar)', 'Notch Raw draws inspiration from rain washing over lush foliage. A vibrant blend of citrus top notes paired with rich woody undertones.'),
    ('notch-celeste-women', 'Notch Celeste Eau De Parfum', 'For Women • Elegant Floral Amber', 'women', 'floral', 119.00, 139.00, 4.9, 512, 'Bestseller', 'https://images.unsplash.com/photo-1541643600914-78b084683601?auto=format&fit=crop&w=800&q=80', 'Mandarin, Green Pear, Grapefruit, Peach', 'Jasmine, Sambac, Waterlily, Orange Blossom', 'White Musk, Patchouli, Sandalwood, Amber', 'Harry Fremont (Qatar)', 'Notch Celeste evokes the carefree joy of a sunny spring afternoon. Vibrant fruity accents blend seamlessly into a rich heart of jasmine.'),
    ('notch-amalfi-bleue', 'Notch Amalfi Bleue EDP', 'Unisex • Mediterranean Aquatic Fresh', 'unisex', 'oceanic', 139.00, 169.00, 4.9, 380, 'Trending', 'https://images.unsplash.com/photo-1523293182086-7651a899d37f?auto=format&fit=crop&w=800&q=80', 'Citrus Zest, Apple, Sea Breeze Accord', 'Clary Sage, Violet Leaf, Fig Tree', 'Ambergris, Driftwood, Vetiver', 'Jordi Fernandez (Qatar)', 'Transport yourself to the sun-drenched cliffs of the Italian coastline. Fresh ocean breezes meet aromatic Mediterranean herbs.'),
    ('notch-steele-men', 'Notch Steele Eau De Parfum', 'For Men • Intense Spiced Leather & Wood', 'men', 'woody', 119.00, 129.00, 4.7, 295, 'Popular', 'https://images.unsplash.com/photo-1508746829417-e6f548d8d6ed?auto=format&fit=crop&w=800&q=80', 'Pink Pepper, Bergamot, Cardamom', 'Nutmeg, Pimento, Cistus', 'Smoky Leather, Cedarwood, Vanilla', 'Fabrice Pellegrin (Qatar)', 'Notch Steele embodies charisma and strength. A warm, spicy heart layered with opulent leather and smoky woods.');
END;
GO

-- Seed Admin User
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, Email, Password, Role) VALUES
    ('admin', 'admin@notchperfumes.com', 'Admin@123', 'Admin');
END;
GO

-- Create SubCategories Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SubCategories')
BEGIN
    CREATE TABLE SubCategories (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        MainCategoryId NVARCHAR(50) NOT NULL,
        SubCategoryName NVARCHAR(100) NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Create Banners Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Banners')
BEGIN
    CREATE TABLE Banners (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(150) NOT NULL,
        Subtitle NVARCHAR(250),
        Image NVARCHAR(MAX) NOT NULL,
        TargetUrl NVARCHAR(250),
        IsActive BIT DEFAULT 1,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Create CategorySpecifications Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CategorySpecifications')
BEGIN
    CREATE TABLE CategorySpecifications (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CategoryId NVARCHAR(50) NOT NULL,
        SpecName NVARCHAR(100) NOT NULL,
        SpecValues NVARCHAR(MAX),
        IsRequired BIT DEFAULT 0,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Create Suppliers Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Suppliers')
BEGIN
    CREATE TABLE Suppliers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SupplierName NVARCHAR(150) NOT NULL,
        ContactPerson NVARCHAR(100),
        Email NVARCHAR(150),
        Phone NVARCHAR(50),
        Address NVARCHAR(MAX),
        Status NVARCHAR(50) DEFAULT 'Active',
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END;
GO

-- Create Customers Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
BEGIN
    CREATE TABLE Customers (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CustomerName NVARCHAR(150) NOT NULL,
        Email NVARCHAR(150),
        Phone NVARCHAR(50),
        City NVARCHAR(100),
        TotalOrders INT DEFAULT 0,
        TotalSpent DECIMAL(18, 2) DEFAULT 0,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END;
GO


