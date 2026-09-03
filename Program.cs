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

// Redirect deleted admin-categories.html to admin-dashboard.html
app.MapGet("/admin-categories.html", () => Results.Redirect("/admin-dashboard.html"));

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

app.Run();
