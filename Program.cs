using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using CoffeeCo.Data;
using DotNetEnv;

// Load .env file
var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine("Loaded .env file");
}
else
{
    Console.WriteLine("Warning: .env file not found. Using environment variables.");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure MySQL Database
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";

if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbName) || 
    string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
{
    throw new Exception("Missing required database environment variables. Please check your .env file.");
}

// Build connection string with proper SSL settings for RDS
// Pomelo MySQL format - try with SSL but allow certificate validation issues
var connectionString = $"Server={dbHost};Port={dbPort};Database={dbName};User={dbUser};Password={dbPassword};SslMode=Required;AllowUserVariables=True;";

Console.WriteLine($"Configuring database connection:");
Console.WriteLine($"  Host: {dbHost}");
Console.WriteLine($"  Database: {dbName}");
Console.WriteLine($"  User: {dbUser}");
Console.WriteLine($"  Port: {dbPort}");

// Add DbContext
builder.Services.AddDbContext<CoffeeDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.21-mysql"), mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    });
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

Console.WriteLine("Database context configured.");

var app = builder.Build();

// Configure the HTTP request pipeline

app.UseCors("AllowAll");
app.UseAuthorization();

// Serve static files from public directory
var publicPath = Path.Combine(builder.Environment.ContentRootPath, "public");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(publicPath),
    RequestPath = ""
});

// Map API controllers FIRST (before fallback)
app.MapControllers();

// Serve index.html for all other routes (SPA fallback) - but NOT for /api routes
app.MapFallback(async context =>
{
    // Only serve index.html for non-API routes
    if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.ContentType = "text/html";
        var filePath = Path.Combine(publicPath, "index.html");
        if (File.Exists(filePath))
        {
            await context.Response.SendFileAsync(filePath);
        }
        else
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("File not found");
        }
    }
    else
    {
        // If it's an API route that wasn't matched, return 404
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("API endpoint not found");
    }
});

// Ensure database is created (non-blocking, don't block server startup)
_ = Task.Run(async () =>
{
    try
    {
        await Task.Delay(2000); // Wait for the app to fully start
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CoffeeDbContext>();
            Console.WriteLine("Ensuring database is created...");
            try
            {
                dbContext.Database.EnsureCreated();
                Console.WriteLine("Database ready.");
            }
            catch (Exception dbEx)
            {
                Console.WriteLine($"Warning: Could not ensure database is created: {dbEx.Message}");
                Console.WriteLine("The server will continue, but database operations may fail.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in database initialization task: {ex.Message}");
    }
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
var url = $"http://localhost:{port}";

Console.WriteLine($"Starting server on {url}...");
Console.WriteLine("Press Ctrl+C to stop the server.");

try
{
    app.Run(url);
}
catch (Exception ex)
{
    Console.WriteLine($"Fatal error starting server: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}

