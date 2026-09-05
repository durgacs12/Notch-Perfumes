using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "."
});

var app = builder.Build();

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=38.247.130.125;Database=Notchperfumesdb;User Id=sa;Password=Emcon2!@#;TrustServerCertificate=True;Encrypt=False;";

IDbConnection GetConnection() => new SqlConnection(connectionString);

// Auto-create database tables on startup if missing
try
{
    using var initDb = GetConnection();
    initDb.Execute(@"
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
        BEGIN
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'CategoryName')
            BEGIN
                ALTER TABLE Categories ADD CategoryName NVARCHAR(100);
            END;
        END;
        ELSE
        BEGIN
            CREATE TABLE Categories (
                Id NVARCHAR(50) PRIMARY KEY,
                Name NVARCHAR(100),
                CategoryName NVARCHAR(100)
            );
        END;

        -- Ensure exact 7 Storefront categories exist in Categories table
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 'men' OR CategoryName = 'MEN' OR Name = 'MEN')
            INSERT INTO Categories (Id, Name, CategoryName) VALUES ('men', 'MEN', 'MEN');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 'women' OR CategoryName = 'WOMEN' OR Name = 'WOMEN')
            INSERT INTO Categories (Id, Name, CategoryName) VALUES ('women', 'WOMEN', 'WOMEN');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 'collections' OR CategoryName = 'COLLECTIONS' OR Name = 'COLLECTIONS')
            INSERT INTO Categories (Id, Name, CategoryName) VALUES ('collections', 'COLLECTIONS', 'COLLECTIONS');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 'gifting' OR CategoryName = 'GIFTING' OR Name = 'GIFTING')
            INSERT INTO Categories (Id, Name, CategoryName) VALUES ('gifting', 'GIFTING', 'GIFTING');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 'fragrances' OR CategoryName = 'FRAGRANCES' OR Name = 'FRAGRANCES')
            INSERT INTO Categories (Id, Name, CategoryName) VALUES ('fragrances', 'FRAGRANCES', 'FRAGRANCES');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 'notch-blog' OR CategoryName = 'NOTCH BLOG' OR Name = 'NOTCH BLOG')
            INSERT INTO Categories (Id, Name, CategoryName) VALUES ('notch-blog', 'NOTCH BLOG', 'NOTCH BLOG');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = 'sale' OR CategoryName = 'SALE' OR Name = 'SALE')
            INSERT INTO Categories (Id, Name, CategoryName) VALUES ('sale', 'SALE', 'SALE');

        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SubCategories')
        BEGIN
            CREATE TABLE SubCategories (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                MainCategoryId NVARCHAR(50) NOT NULL,
                SubCategoryName NVARCHAR(100) NOT NULL,
                CreatedAt DATETIME DEFAULT GETDATE()
            );
        END;

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

        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customers')
        BEGIN
            CREATE TABLE Customers (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                FirstName NVARCHAR(100) NOT NULL,
                LastName NVARCHAR(100),
                Email NVARCHAR(150) NOT NULL,
                MobileNumber NVARCHAR(50),
                Gender NVARCHAR(20),
                Address NVARCHAR(MAX),
                City NVARCHAR(100),
                Postcode NVARCHAR(50),
                Password NVARCHAR(100),
                CreatedAt DATETIME DEFAULT GETDATE()
            );
        END;

        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Purchases')
        BEGIN
            CREATE TABLE Purchases (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                PurchaseNo NVARCHAR(50) NOT NULL,
                PurchaseDate DATETIME NOT NULL,
                BatchNo NVARCHAR(50) NOT NULL,
                SupplierId INT NULL,
                SupplierName NVARCHAR(150) NOT NULL,
                TotalAmount DECIMAL(18, 2) DEFAULT 0,
                CreatedAt DATETIME DEFAULT GETDATE()
            );
        END;

        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PurchaseItems')
        BEGIN
            CREATE TABLE PurchaseItems (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                PurchaseId INT NOT NULL,
                PurchaseNo NVARCHAR(50),
                ProductName NVARCHAR(150) NOT NULL,
                ProductCode NVARCHAR(50),
                Variant NVARCHAR(50),
                VariantCode NVARCHAR(50),
                CostPrice DECIMAL(18, 2) DEFAULT 0,
                DiscPercent DECIMAL(18, 2) DEFAULT 0,
                DiscAmt DECIMAL(18, 2) DEFAULT 0,
                Quantity INT DEFAULT 1,
                GstPercent DECIMAL(18, 2) DEFAULT 0,
                GstAmt DECIMAL(18, 2) DEFAULT 0,
                TotalAmount DECIMAL(18, 2) DEFAULT 0,
                SellingPrice DECIMAL(18, 2) DEFAULT 0
            );
        END;
    ");
}
catch (Exception ex)
{
    Console.WriteLine($"DB Auto-Create Warning: {ex.Message}");
}

app.UseDefaultFiles();
app.UseStaticFiles();

// Ensure existing Suppliers table in SQL Server has all Notch Perfumes project columns
try
{
    using var migrationDb = GetConnection();
    migrationDb.Execute(@"
        IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Suppliers')
        BEGIN
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'SupplierCode')
                ALTER TABLE Suppliers ADD SupplierCode NVARCHAR(50);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'Type')
                ALTER TABLE Suppliers ADD Type NVARCHAR(50);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'Country')
                ALTER TABLE Suppliers ADD Country NVARCHAR(100) DEFAULT 'India';
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'State')
                ALTER TABLE Suppliers ADD State NVARCHAR(100);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'City')
                ALTER TABLE Suppliers ADD City NVARCHAR(100);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'PostalCode')
                ALTER TABLE Suppliers ADD PostalCode NVARCHAR(20);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'GSTIN')
                ALTER TABLE Suppliers ADD GSTIN NVARCHAR(50);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'BankName')
                ALTER TABLE Suppliers ADD BankName NVARCHAR(100);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'AccountNumber')
                ALTER TABLE Suppliers ADD AccountNumber NVARCHAR(50);
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Suppliers') AND name = 'IFSC')
                ALTER TABLE Suppliers ADD IFSC NVARCHAR(50);
        END;

        -- Clean any legacy dummy sample products
        DELETE FROM OrderItems WHERE ProductId IN ('notch-oud-royale', 'notch-raw-men', 'notch-celeste-women', 'notch-amalfi-bleue', 'notch-steele-men', 'notch-nox-him', 'notch-noura-her', 'notch-discovery-kit', 'notch-nude-women');
        DELETE FROM Products WHERE Id IN ('notch-oud-royale', 'notch-raw-men', 'notch-celeste-women', 'notch-amalfi-bleue', 'notch-steele-men', 'notch-nox-him', 'notch-noura-her', 'notch-discovery-kit', 'notch-nude-women');
    ");
}
catch (Exception ex)
{
    Console.WriteLine($"Supplier table migration note: {ex.Message}");
}

// API Endpoints for NotchPerfumes Database

// Get All Products
app.MapGet("/api/products", async () =>
{
    using var db = GetConnection();
    var products = await db.QueryAsync("SELECT * FROM Products ORDER BY CreatedAt DESC");
    return Results.Ok(products);
});

// Get Product by ID
app.MapGet("/api/products/{id}", async (string id) =>
{
    using var db = GetConnection();
    var product = await db.QueryFirstOrDefaultAsync("SELECT * FROM Products WHERE Id = @Id", new { Id = id });
    return product is not null ? Results.Ok(product) : Results.NotFound(new { message = "Product not found" });
});

// Create/Update Product
app.MapPost("/api/products", async (ProductDto dto) =>
{
    using var db = GetConnection();
    string id = string.IsNullOrWhiteSpace(dto.Id) ? $"notch-custom-{DateTime.UtcNow.Ticks}" : dto.Id;
    string sql = @"IF EXISTS (SELECT 1 FROM Products WHERE Id = @Id)
                   BEGIN
                       UPDATE Products SET Name=@Name, Subtitle=@Subtitle, Category=@Category, ScentFamily=@ScentFamily, Price=@Price, OriginalPrice=@OriginalPrice, Image=@Image, Description=@Description, TopNotes=@TopNotes, HeartNotes=@HeartNotes, BaseNotes=@BaseNotes, Perfumer=@Perfumer WHERE Id=@Id;
                   END
                   ELSE
                   BEGIN
                       INSERT INTO Products (Id, Name, Subtitle, Category, ScentFamily, Price, OriginalPrice, Image, Description, TopNotes, HeartNotes, BaseNotes, Perfumer)
                       VALUES (@Id, @Name, @Subtitle, @Category, @ScentFamily, @Price, @OriginalPrice, @Image, @Description, @TopNotes, @HeartNotes, @BaseNotes, @Perfumer);
                   END";

    await db.ExecuteAsync(sql, new
    {
        Id = id,
        Name = dto.Name ?? "Perfume Product",
        Subtitle = dto.Subtitle ?? "",
        Category = dto.Category ?? "unisex",
        ScentFamily = dto.ScentFamily ?? "amber",
        Price = dto.Price,
        OriginalPrice = dto.OriginalPrice > 0 ? dto.OriginalPrice : Math.Round(dto.Price * 1.25m),
        Image = dto.Image ?? "",
        Description = dto.Description ?? "",
        TopNotes = dto.TopNotes ?? "",
        HeartNotes = dto.HeartNotes ?? "",
        BaseNotes = dto.BaseNotes ?? "",
        Perfumer = dto.Perfumer ?? ""
    });

    return Results.Ok(new { Id = id, Name = dto.Name });
});

// Delete Product
app.MapDelete("/api/products/{id}", async (string id) =>
{
    using var db = GetConnection();
    int rows = await db.ExecuteAsync("DELETE FROM Products WHERE Id = @Id", new { Id = id });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

// Categories Endpoints
app.MapGet("/api/categories", async () =>
{
    try
    {
        using var db = GetConnection();
        var categories = await db.QueryAsync("SELECT Id, COALESCE(CategoryName, Name) AS CategoryName, COALESCE(Name, CategoryName) AS Name FROM Categories");
        return Results.Ok(categories);
    }
    catch (Exception)
    {
        return Results.Ok(new List<object>());
    }
});

app.MapPost("/api/categories", async (CategoryDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.CategoryName))
        return Results.BadRequest(new { message = "CategoryName is required." });

    using var db = GetConnection();
    string id = string.IsNullOrWhiteSpace(dto.Id) ? dto.CategoryName.Trim().ToLower().Replace(" ", "-").Replace("'", "") : dto.Id;
    string sql = @"IF EXISTS (SELECT 1 FROM Categories WHERE Id = @Id)
                   BEGIN
                       UPDATE Categories SET Name = @CategoryName, CategoryName = @CategoryName WHERE Id = @Id;
                   END
                   ELSE
                   BEGIN
                       INSERT INTO Categories (Id, Name, CategoryName)
                       VALUES (@Id, @CategoryName, @CategoryName);
                   END";
    await db.ExecuteAsync(sql, new { Id = id, dto.CategoryName });
    return Results.Ok(new { Id = id, dto.CategoryName, Name = dto.CategoryName });
});

app.MapPut("/api/categories/{id}", async (string id, CategoryDto dto) =>
{
    using var db = GetConnection();
    string sql = @"UPDATE Categories SET Name = @CategoryName, CategoryName = @CategoryName WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sql, new { Id = id, dto.CategoryName });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

app.MapDelete("/api/categories/{id}", async (string id) =>
{
    using var db = GetConnection();
    int rows = await db.ExecuteAsync("DELETE FROM Categories WHERE Id = @Id OR Name = @Id OR CategoryName = @Id", new { Id = id });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

// Get SubCategories
app.MapGet("/api/subcategories", async () =>
{
    using var db = GetConnection();
    var subcategories = await db.QueryAsync("SELECT * FROM SubCategories ORDER BY Id ASC");
    return Results.Ok(subcategories);
});

// Create SubCategory
app.MapPost("/api/subcategories", async (SubCategoryDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.MainCategoryId) || string.IsNullOrWhiteSpace(dto.SubCategoryName))
        return Results.BadRequest(new { message = "MainCategoryId and SubCategoryName are required." });

    using var db = GetConnection();
    string sql = @"INSERT INTO SubCategories (MainCategoryId, SubCategoryName)
                   VALUES (@MainCategoryId, @SubCategoryName);
                   SELECT CAST(SCOPE_IDENTITY() as int);";
    int id = await db.ExecuteScalarAsync<int>(sql, dto);
    return Results.Created($"/api/subcategories/{id}", new { Id = id, dto.MainCategoryId, dto.SubCategoryName });
});

// Update SubCategory
app.MapPut("/api/subcategories/{id:int}", async (int id, SubCategoryDto dto) =>
{
    using var db = GetConnection();
    string sql = @"UPDATE SubCategories SET MainCategoryId = @MainCategoryId, SubCategoryName = @SubCategoryName WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sql, new { Id = id, dto.MainCategoryId, dto.SubCategoryName });
    return rows > 0 ? Results.Ok(new { message = "Updated successfully" }) : Results.NotFound();
});

// Delete SubCategory
app.MapDelete("/api/subcategories/{id:int}", async (int id) =>
{
    using var db = GetConnection();
    int rows = await db.ExecuteAsync("DELETE FROM SubCategories WHERE Id = @Id", new { Id = id });
    return rows > 0 ? Results.Ok(new { message = "Deleted successfully" }) : Results.NotFound();
});


// Get Orders (with Items)
app.MapGet("/api/orders", async () =>
{
    try
    {
        using var db = GetConnection();
        var orders = (await db.QueryAsync<dynamic>("SELECT * FROM Orders ORDER BY CreatedAt DESC")).ToList();
        var items = (await db.QueryAsync<dynamic>("SELECT * FROM OrderItems")).ToList();

        var result = orders.Select(o => new
        {
            id = (int)o.Id,
            orderNumber = (string)o.OrderNumber,
            customerName = (string)o.CustomerName,
            customerEmail = o.CustomerEmail != null ? (string)o.CustomerEmail : "",
            customerPhone = o.CustomerPhone != null ? (string)o.CustomerPhone : "",
            shippingAddress = o.ShippingAddress != null ? (string)o.ShippingAddress : "",
            totalAmount = (decimal)o.TotalAmount,
            paymentMethod = o.PaymentMethod != null ? (string)o.PaymentMethod : "COD",
            status = o.Status != null ? (string)o.Status : "Pending",
            createdAt = (DateTime)o.CreatedAt,
            items = items.Where(i => (int)i.OrderId == (int)o.Id).Select(i => new {
                id = (int)i.Id,
                orderId = (int)i.OrderId,
                productId = i.ProductId != null ? (string)i.ProductId : "",
                productName = (string)i.ProductName,
                quantity = (int)i.Quantity,
                unitPrice = (decimal)i.UnitPrice,
                totalPrice = (decimal)i.TotalPrice
            }).ToList()
        });

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

// Get Order by ID
app.MapGet("/api/orders/{id:int}", async (int id) =>
{
    using var db = GetConnection();
    var order = await db.QueryFirstOrDefaultAsync<dynamic>("SELECT * FROM Orders WHERE Id = @Id", new { Id = id });
    if (order is null) return Results.NotFound();

    var items = await db.QueryAsync<dynamic>("SELECT * FROM OrderItems WHERE OrderId = @Id", new { Id = id });

    return Results.Ok(new
    {
        id = (int)order.Id,
        orderNumber = (string)order.OrderNumber,
        customerName = (string)order.CustomerName,
        customerEmail = order.CustomerEmail != null ? (string)order.CustomerEmail : "",
        customerPhone = order.CustomerPhone != null ? (string)order.CustomerPhone : "",
        shippingAddress = order.ShippingAddress != null ? (string)order.ShippingAddress : "",
        totalAmount = (decimal)order.TotalAmount,
        paymentMethod = order.PaymentMethod != null ? (string)order.PaymentMethod : "COD",
        status = order.Status != null ? (string)order.Status : "Pending",
        createdAt = (DateTime)order.CreatedAt,
        items
    });
});

// Create Order (with Items)
app.MapPost("/api/orders", async (OrderCreateDto dto) =>
{
    using var db = GetConnection();
    string sqlOrder = @"INSERT INTO Orders (OrderNumber, CustomerName, CustomerEmail, CustomerPhone, ShippingAddress, TotalAmount, PaymentMethod, Status)
                        VALUES (@OrderNumber, @CustomerName, @CustomerEmail, @CustomerPhone, @ShippingAddress, @TotalAmount, @PaymentMethod, @Status);
                        SELECT CAST(SCOPE_IDENTITY() as int);";

    int orderId = await db.ExecuteScalarAsync<int>(sqlOrder, new
    {
        dto.OrderNumber,
        CustomerName = dto.CustomerName ?? "Customer",
        CustomerEmail = dto.CustomerEmail ?? "",
        CustomerPhone = dto.CustomerPhone ?? "",
        ShippingAddress = dto.ShippingAddress ?? "",
        dto.TotalAmount,
        PaymentMethod = dto.PaymentMethod ?? "COD",
        Status = dto.Status ?? "Pending"
    });

    if (dto.Items != null && dto.Items.Any())
    {
        string sqlItem = @"INSERT INTO OrderItems (OrderId, ProductId, ProductName, Quantity, UnitPrice, TotalPrice)
                           VALUES (@OrderId, @ProductId, @ProductName, @Quantity, @UnitPrice, @TotalPrice);";
        foreach (var item in dto.Items)
        {
            await db.ExecuteAsync(sqlItem, new
            {
                OrderId = orderId,
                ProductId = item.ProductId ?? "",
                ProductName = item.ProductName ?? "Product",
                Quantity = item.Quantity > 0 ? item.Quantity : 1,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice > 0 ? item.TotalPrice : (item.UnitPrice * item.Quantity)
            });
        }
    }

    return Results.Created($"/api/orders/{orderId}", new { Id = orderId, OrderNumber = dto.OrderNumber });
});

// Update Order Status
app.MapPut("/api/orders/{id:int}/status", async (int id, OrderStatusDto dto) =>
{
    using var db = GetConnection();
    string sql = @"UPDATE Orders SET Status = @Status WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sql, new { Id = id, Status = dto.Status });
    return rows > 0 ? Results.Ok(new { message = "Order status updated" }) : Results.NotFound();
});

// Get Coupons
app.MapGet("/api/coupons", async () =>
{
    using var db = GetConnection();
    var coupons = await db.QueryAsync("SELECT * FROM Coupons WHERE IsActive = 1");
    return Results.Ok(coupons);
});

// Database Health Check Endpoint
app.MapGet("/api/health", async () =>
{
    try
    {
        using var db = GetConnection();
        var result = await db.ExecuteScalarAsync<int>("SELECT 1");
        return Results.Ok(new { status = "Online", database = "Notchperfumesdb", server = "38.247.130.125" });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

// Banners Endpoints
app.MapGet("/api/banners", async () =>
{
    using var db = GetConnection();
    var banners = await db.QueryAsync("SELECT * FROM Banners ORDER BY Id DESC");
    return Results.Ok(banners);
});

app.MapPost("/api/banners", async (BannerDto dto) =>
{
    using var db = GetConnection();
    string sql = @"INSERT INTO Banners (Title, Subtitle, Image, TargetUrl, IsActive)
                   VALUES (@Title, @Subtitle, @Image, @TargetUrl, @IsActive);
                   SELECT CAST(SCOPE_IDENTITY() as int);";
    int id = await db.ExecuteScalarAsync<int>(sql, dto);
    return Results.Created($"/api/banners/{id}", new { Id = id, dto.Title, dto.Subtitle, dto.Image, dto.TargetUrl, dto.IsActive });
});

app.MapPut("/api/banners/{id:int}", async (int id, BannerDto dto) =>
{
    using var db = GetConnection();
    string sql = @"UPDATE Banners SET Title = @Title, Subtitle = @Subtitle, Image = @Image, TargetUrl = @TargetUrl, IsActive = @IsActive WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sql, new { Id = id, dto.Title, dto.Subtitle, dto.Image, dto.TargetUrl, dto.IsActive });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

app.MapDelete("/api/banners/{id:int}", async (int id) =>
{
    using var db = GetConnection();
    int rows = await db.ExecuteAsync("DELETE FROM Banners WHERE Id = @Id", new { Id = id });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

// CategorySpecifications Endpoints
app.MapGet("/api/categoryspecifications", async () =>
{
    using var db = GetConnection();
    var specs = await db.QueryAsync("SELECT * FROM CategorySpecifications ORDER BY Id ASC");
    return Results.Ok(specs);
});

app.MapPost("/api/categoryspecifications", async (CategorySpecDto dto) =>
{
    using var db = GetConnection();
    string sql = @"INSERT INTO CategorySpecifications (CategoryId, SpecName, SpecValues, IsRequired)
                   VALUES (@CategoryId, @SpecName, @SpecValues, @IsRequired);
                   SELECT CAST(SCOPE_IDENTITY() as int);";
    int id = await db.ExecuteScalarAsync<int>(sql, dto);
    return Results.Created($"/api/categoryspecifications/{id}", new { Id = id, dto.CategoryId, dto.SpecName, dto.SpecValues, dto.IsRequired });
});

app.MapPut("/api/categoryspecifications/{id:int}", async (int id, CategorySpecDto dto) =>
{
    using var db = GetConnection();
    string sql = @"UPDATE CategorySpecifications SET CategoryId = @CategoryId, SpecName = @SpecName, SpecValues = @SpecValues, IsRequired = @IsRequired WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sql, new { Id = id, dto.CategoryId, dto.SpecName, dto.SpecValues, dto.IsRequired });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

app.MapDelete("/api/categoryspecifications/{id:int}", async (int id) =>
{
    using var db = GetConnection();
    int rows = await db.ExecuteAsync("DELETE FROM CategorySpecifications WHERE Id = @Id", new { Id = id });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

// Suppliers Endpoints
app.MapGet("/api/suppliers", async () =>
{
    using var db = GetConnection();
    var suppliers = await db.QueryAsync("SELECT * FROM Suppliers ORDER BY Id DESC");
    return Results.Ok(suppliers);
});

app.MapPost("/api/suppliers", async (SupplierDto dto) =>
{
    using var db = GetConnection();
    string sql = @"INSERT INTO Suppliers (SupplierCode, SupplierName, Type, Status, Country, State, City, Address, PostalCode, Phone, Email, GSTIN, BankName, AccountNumber, IFSC)
                   VALUES (@SupplierCode, @SupplierName, @Type, @Status, @Country, @State, @City, @Address, @PostalCode, @Phone, @Email, @GSTIN, @BankName, @AccountNumber, @IFSC);
                   SELECT CAST(SCOPE_IDENTITY() as int);";
    int id = await db.ExecuteScalarAsync<int>(sql, dto);
    return Results.Created($"/api/suppliers/{id}", dto with { Id = id });
});

app.MapPut("/api/suppliers/{id:int}", async (int id, SupplierDto dto) =>
{
    using var db = GetConnection();
    string sql = @"UPDATE Suppliers SET SupplierCode = @SupplierCode, SupplierName = @SupplierName, Type = @Type, Status = @Status, 
                   Country = @Country, State = @State, City = @City, Address = @Address, PostalCode = @PostalCode, 
                   Phone = @Phone, Email = @Email, GSTIN = @GSTIN, BankName = @BankName, AccountNumber = @AccountNumber, IFSC = @IFSC 
                   WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sql, new { 
        Id = id, 
        dto.SupplierCode, 
        dto.SupplierName, 
        dto.Type, 
        dto.Status, 
        dto.Country, 
        dto.State, 
        dto.City, 
        dto.Address, 
        dto.PostalCode, 
        dto.Phone, 
        dto.Email, 
        dto.GSTIN, 
        dto.BankName, 
        dto.AccountNumber, 
        dto.IFSC 
    });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

app.MapDelete("/api/suppliers/{id:int}", async (int id) =>
{
    using var db = GetConnection();
    int rows = await db.ExecuteAsync("DELETE FROM Suppliers WHERE Id = @Id", new { Id = id });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

// Customers Endpoints
app.MapGet("/api/customers", async () =>
{
    try
    {
        using var db = GetConnection();
        var customers = await db.QueryAsync("SELECT * FROM Customers ORDER BY Id DESC");
        return Results.Ok(customers);
    }
    catch (Exception)
    {
        return Results.Ok(new List<object>());
    }
});

app.MapPost("/api/customers", async (CustomerDto dto) =>
{
    using var db = GetConnection();
    string sql = @"INSERT INTO Customers (FirstName, LastName, Email, MobileNumber, Gender, Address, City, Postcode, Password)
                   VALUES (@FirstName, @LastName, @Email, @MobileNumber, @Gender, @Address, @City, @Postcode, @Password);
                   SELECT CAST(SCOPE_IDENTITY() as int);";
    int id = await db.ExecuteScalarAsync<int>(sql, dto);
    return Results.Created($"/api/customers/{id}", new { Id = id, dto.FirstName, dto.LastName, dto.Email, dto.MobileNumber, dto.Gender, dto.Address, dto.City, dto.Postcode });
});

app.MapPut("/api/customers/{id:int}", async (int id, CustomerDto dto) =>
{
    using var db = GetConnection();
    string sql = @"UPDATE Customers SET FirstName = @FirstName, LastName = @LastName, Email = @Email, MobileNumber = @MobileNumber, Gender = @Gender, Address = @Address, City = @City, Postcode = @Postcode, Password = @Password WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sql, new { Id = id, dto.FirstName, dto.LastName, dto.Email, dto.MobileNumber, dto.Gender, dto.Address, dto.City, dto.Postcode, dto.Password });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

app.MapDelete("/api/customers/{id:int}", async (int id) =>
{
    using var db = GetConnection();
    int rows = await db.ExecuteAsync("DELETE FROM Customers WHERE Id = @Id", new { Id = id });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

// Purchases Endpoints
app.MapGet("/api/purchases", async () =>
{
    try
    {
        using var db = GetConnection();
        var purchases = (await db.QueryAsync<dynamic>("SELECT * FROM Purchases ORDER BY Id DESC")).ToList();
        var items = (await db.QueryAsync<dynamic>("SELECT * FROM PurchaseItems")).ToList();

        var result = purchases.Select(p => new
        {
            id = (int)p.Id,
            purchaseNo = (string)p.PurchaseNo,
            purchaseDate = (DateTime)p.PurchaseDate,
            batchNo = (string)p.BatchNo,
            supplierId = p.SupplierId != null ? (int?)p.SupplierId : null,
            supplierName = (string)p.SupplierName,
            totalAmount = (decimal)p.TotalAmount,
            createdAt = (DateTime)p.CreatedAt,
            items = items.Where(i => (int)i.PurchaseId == (int)p.Id).Select(i => new {
                id = (int)i.Id,
                purchaseId = (int)i.PurchaseId,
                purchaseNo = (string)i.PurchaseNo,
                productName = (string)i.ProductName,
                productCode = (string)i.ProductCode,
                variant = (string)i.Variant,
                variantCode = (string)i.VariantCode,
                costPrice = (decimal)i.CostPrice,
                discPercent = (decimal)i.DiscPercent,
                discAmt = (decimal)i.DiscAmt,
                quantity = (int)i.Quantity,
                gstPercent = (decimal)i.GstPercent,
                gstAmt = (decimal)i.GstAmt,
                totalAmount = (decimal)i.TotalAmount,
                sellingPrice = (decimal)i.SellingPrice
            }).ToList()
        });

        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("/api/purchases/{id:int}", async (int id) =>
{
    using var db = GetConnection();
    var purchase = await db.QueryFirstOrDefaultAsync<dynamic>("SELECT * FROM Purchases WHERE Id = @Id", new { Id = id });
    if (purchase is null) return Results.NotFound();

    var items = await db.QueryAsync<dynamic>("SELECT * FROM PurchaseItems WHERE PurchaseId = @Id", new { Id = id });

    return Results.Ok(new
    {
        id = (int)purchase.Id,
        purchaseNo = (string)purchase.PurchaseNo,
        purchaseDate = (DateTime)purchase.PurchaseDate,
        batchNo = (string)purchase.BatchNo,
        supplierId = purchase.SupplierId != null ? (int?)purchase.SupplierId : null,
        supplierName = (string)purchase.SupplierName,
        totalAmount = (decimal)purchase.TotalAmount,
        createdAt = (DateTime)purchase.CreatedAt,
        items
    });
});

app.MapPost("/api/purchases", async (PurchaseDto dto) =>
{
    using var db = GetConnection();
    string sqlPur = @"INSERT INTO Purchases (PurchaseNo, PurchaseDate, BatchNo, SupplierId, SupplierName, TotalAmount)
                      VALUES (@PurchaseNo, @PurchaseDate, @BatchNo, @SupplierId, @SupplierName, @TotalAmount);
                      SELECT CAST(SCOPE_IDENTITY() as int);";
    int purchaseId = await db.ExecuteScalarAsync<int>(sqlPur, dto);

    if (dto.Items != null && dto.Items.Any())
    {
        string sqlItem = @"INSERT INTO PurchaseItems (PurchaseId, PurchaseNo, ProductName, ProductCode, Variant, VariantCode, CostPrice, DiscPercent, DiscAmt, Quantity, GstPercent, GstAmt, TotalAmount, SellingPrice)
                           VALUES (@PurchaseId, @PurchaseNo, @ProductName, @ProductCode, @Variant, @VariantCode, @CostPrice, @DiscPercent, @DiscAmt, @Quantity, @GstPercent, @GstAmt, @TotalAmount, @SellingPrice);";
        foreach (var item in dto.Items)
        {
            await db.ExecuteAsync(sqlItem, new
            {
                PurchaseId = purchaseId,
                PurchaseNo = dto.PurchaseNo,
                item.ProductName,
                item.ProductCode,
                item.Variant,
                item.VariantCode,
                item.CostPrice,
                item.DiscPercent,
                item.DiscAmt,
                item.Quantity,
                item.GstPercent,
                item.GstAmt,
                item.TotalAmount,
                item.SellingPrice
            });
        }
    }

    return Results.Created($"/api/purchases/{purchaseId}", dto with { Id = purchaseId });
});

app.MapPut("/api/purchases/{id:int}", async (int id, PurchaseDto dto) =>
{
    using var db = GetConnection();
    string sqlPur = @"UPDATE Purchases SET PurchaseNo = @PurchaseNo, PurchaseDate = @PurchaseDate, BatchNo = @BatchNo, 
                      SupplierId = @SupplierId, SupplierName = @SupplierName, TotalAmount = @TotalAmount 
                      WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sqlPur, new { Id = id, dto.PurchaseNo, dto.PurchaseDate, dto.BatchNo, dto.SupplierId, dto.SupplierName, dto.TotalAmount });
    if (rows == 0) return Results.NotFound();

    await db.ExecuteAsync("DELETE FROM PurchaseItems WHERE PurchaseId = @Id", new { Id = id });

    if (dto.Items != null && dto.Items.Any())
    {
        string sqlItem = @"INSERT INTO PurchaseItems (PurchaseId, PurchaseNo, ProductName, ProductCode, Variant, VariantCode, CostPrice, DiscPercent, DiscAmt, Quantity, GstPercent, GstAmt, TotalAmount, SellingPrice)
                           VALUES (@PurchaseId, @PurchaseNo, @ProductName, @ProductCode, @Variant, @VariantCode, @CostPrice, @DiscPercent, @DiscAmt, @Quantity, @GstPercent, @GstAmt, @TotalAmount, @SellingPrice);";
        foreach (var item in dto.Items)
        {
            await db.ExecuteAsync(sqlItem, new
            {
                PurchaseId = id,
                PurchaseNo = dto.PurchaseNo,
                item.ProductName,
                item.ProductCode,
                item.Variant,
                item.VariantCode,
                item.CostPrice,
                item.DiscPercent,
                item.DiscAmt,
                item.Quantity,
                item.GstPercent,
                item.GstAmt,
                item.TotalAmount,
                item.SellingPrice
            });
        }
    }

    return Results.Ok(new { message = "Purchase updated successfully" });
});

app.MapDelete("/api/purchases/{id:int}", async (int id) =>
{
    using var db = GetConnection();
    await db.ExecuteAsync("DELETE FROM PurchaseItems WHERE PurchaseId = @Id", new { Id = id });
    int rows = await db.ExecuteAsync("DELETE FROM Purchases WHERE Id = @Id", new { Id = id });
    return rows > 0 ? Results.Ok(new { message = "Purchase deleted successfully" }) : Results.NotFound();
});

app.Run();

public record SubCategoryDto(int? Id, string MainCategoryId, string SubCategoryName);
public record BannerDto(int? Id, string Title, string Subtitle, string Image, string TargetUrl, bool IsActive);
public record CategorySpecDto(int? Id, string CategoryId, string SpecName, string SpecValues, bool IsRequired);
public record CustomerDto(int? Id, string FirstName, string LastName, string Email, string MobileNumber, string Gender, string Address, string City, string Postcode, string Password);
public record CategoryDto(string? Id, string CategoryName, string? SubCategories, string? Note);
public record SupplierDto(int? Id, string SupplierCode, string SupplierName, string Type, string Status, string Country, string State, string City, string Address, string PostalCode, string Phone, string Email, string GSTIN, string BankName, string AccountNumber, string IFSC);

public record PurchaseItemDto(
    int? Id,
    int? PurchaseId,
    string? PurchaseNo,
    string ProductName,
    string? ProductCode,
    string? Variant,
    string? VariantCode,
    decimal CostPrice,
    decimal DiscPercent,
    decimal DiscAmt,
    int Quantity,
    decimal GstPercent,
    decimal GstAmt,
    decimal TotalAmount,
    decimal SellingPrice
);

public record PurchaseDto(
    int? Id,
    string PurchaseNo,
    DateTime PurchaseDate,
    string BatchNo,
    int? SupplierId,
    string SupplierName,
    decimal TotalAmount,
    List<PurchaseItemDto>? Items
);

public record OrderItemInputDto(string? ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice);
public record OrderCreateDto(string OrderNumber, string? CustomerName, string? CustomerEmail, string? CustomerPhone, string? ShippingAddress, decimal TotalAmount, string? PaymentMethod, string? Status, List<OrderItemInputDto>? Items);
public record OrderStatusDto(string Status);

public record ProductDto(string? Id, string Name, string? Subtitle, string? Category, string? ScentFamily, decimal Price, decimal OriginalPrice, string? Image, string? Description, string? TopNotes, string? HeartNotes, string? BaseNotes, string? Perfumer);





