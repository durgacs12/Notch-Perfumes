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
    ");
}
catch (Exception ex)
{
    Console.WriteLine($"DB Auto-Create Warning: {ex.Message}");
}

app.UseDefaultFiles();
app.UseStaticFiles();

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
    string sql = @"INSERT INTO Suppliers (SupplierName, ContactPerson, Email, Phone, Address, Status)
                   VALUES (@SupplierName, @ContactPerson, @Email, @Phone, @Address, @Status);
                   SELECT CAST(SCOPE_IDENTITY() as int);";
    int id = await db.ExecuteScalarAsync<int>(sql, dto);
    return Results.Created($"/api/suppliers/{id}", new { Id = id, dto.SupplierName, dto.ContactPerson, dto.Email, dto.Phone, dto.Address, dto.Status });
});

app.MapPut("/api/suppliers/{id:int}", async (int id, SupplierDto dto) =>
{
    using var db = GetConnection();
    string sql = @"UPDATE Suppliers SET SupplierName = @SupplierName, ContactPerson = @ContactPerson, Email = @Email, Phone = @Phone, Address = @Address, Status = @Status WHERE Id = @Id";
    int rows = await db.ExecuteAsync(sql, new { Id = id, dto.SupplierName, dto.ContactPerson, dto.Email, dto.Phone, dto.Address, dto.Status });
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

app.Run();

public record SubCategoryDto(int? Id, string MainCategoryId, string SubCategoryName);
public record BannerDto(int? Id, string Title, string Subtitle, string Image, string TargetUrl, bool IsActive);
public record CategorySpecDto(int? Id, string CategoryId, string SpecName, string SpecValues, bool IsRequired);
public record SupplierDto(int? Id, string SupplierName, string ContactPerson, string Email, string Phone, string Address, string Status);
public record CustomerDto(int? Id, string FirstName, string LastName, string Email, string MobileNumber, string Gender, string Address, string City, string Postcode, string Password);



