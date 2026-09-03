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

app.UseDefaultFiles();
app.UseStaticFiles();

// Ensure existing Suppliers table in SQL Server has all MEIL project columns
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
        END
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

// Get Categories
app.MapGet("/api/categories", async () =>
{
    using var db = GetConnection();
    var categories = await db.QueryAsync("SELECT * FROM Categories");
    return Results.Ok(categories);
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


// Get Orders
app.MapGet("/api/orders", async () =>
{
    using var db = GetConnection();
    var orders = await db.QueryAsync("SELECT * FROM Orders ORDER BY CreatedAt DESC");
    return Results.Ok(orders);
});

// Create Order
app.MapPost("/api/orders", async (HttpContext context) =>
{
    using var db = GetConnection();
    var body = await context.Request.ReadFromJsonAsync<dynamic>();
    if (body is null) return Results.BadRequest();

    string sql = @"INSERT INTO Orders (OrderNumber, CustomerName, CustomerEmail, CustomerPhone, ShippingAddress, TotalAmount, Status)
                   VALUES (@OrderNumber, @CustomerName, @CustomerEmail, @CustomerPhone, @ShippingAddress, @TotalAmount, @Status);
                   SELECT CAST(SCOPE_IDENTITY() as int);";

    int id = await db.ExecuteScalarAsync<int>(sql, (object)body);
    return Results.Created($"/api/orders/{id}", new { Id = id });
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
    using var db = GetConnection();
    var customers = await db.QueryAsync("SELECT * FROM Customers ORDER BY Id DESC");
    return Results.Ok(customers);
});

app.MapPost("/api/customers", async (CustomerDto dto) =>
{
    using var db = GetConnection();
    string sql = @"INSERT INTO Customers (CustomerName, Email, Phone, City, TotalOrders, TotalSpent)
                   VALUES (@CustomerName, @Email, @Phone, @City, @TotalOrders, @TotalSpent);
                   SELECT CAST(SCOPE_IDENTITY() as int);";
    int id = await db.ExecuteScalarAsync<int>(sql, dto);
    return Results.Created($"/api/customers/{id}", new { Id = id, dto.CustomerName, dto.Email, dto.Phone, dto.City, dto.TotalOrders, dto.TotalSpent });
});

app.MapPut("/api/customers/{id:int}", async (int id, CustomerDto dto) =>
{
    using var db = GetConnection();
    string sql = @"UPDATE Customers SET CustomerName = @CustomerName, Email = @Email, Phone = @Phone, City = @City, TotalOrders = @TotalOrders, TotalSpent = @TotalSpent WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sql, new { Id = id, dto.CustomerName, dto.Email, dto.Phone, dto.City, dto.TotalOrders, dto.TotalSpent });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

app.MapDelete("/api/customers/{id:int}", async (int id) =>
{
    using var db = GetConnection();
    int rows = await db.ExecuteAsync("DELETE FROM Customers WHERE Id = @Id", new { Id = id });
    return rows > 0 ? Results.Ok() : Results.NotFound();
});

app.Run();

public record SubCategoryDto(int? Id, string MainCategoryId, string SubCategoryName);
public record BannerDto(int? Id, string Title, string Subtitle, string Image, string TargetUrl, bool IsActive);
public record CategorySpecDto(int? Id, string CategoryId, string SpecName, string SpecValues, bool IsRequired);
public record SupplierDto(int? Id, string SupplierCode, string SupplierName, string Type, string Status, string Country, string State, string City, string Address, string PostalCode, string Phone, string Email, string GSTIN, string BankName, string AccountNumber, string IFSC);
public record CustomerDto(int? Id, string CustomerName, string Email, string Phone, string City, int TotalOrders, decimal TotalSpent);


