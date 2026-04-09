using Microsoft.EntityFrameworkCore;
using Serilog;
using FluentValidation;
using FluentValidation.AspNetCore;
using TaxAccount.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using TaxAccount.Authorization;
using System.Text;
using TaxAccount.Services;
using TaxAccount.Middleware;
using TaxAccount.Validators;

// Configure Serilog first before anything else
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/taxaccount-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    Log.Information("Starting TaxAccount API...");

    var builder = WebApplication.CreateBuilder(args);
    // Use Serilog
    builder.Host.UseSerilog();

    // Services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // In-Memory Cache
    builder.Services.AddMemoryCache();

    // FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

    // Product Service
    builder.Services.AddScoped<IProductService, ProductService>();

    // Database
     builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // After other service registrations - authentication services
    builder.Services.AddScoped<IAuthService, AuthService>();

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"]!;

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
    options.TokenValidationParameters = new TokenValidationParameters
        {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
        };
    });

    // Register permission handler
    builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

    // Register each permission as a policy
    builder.Services.AddAuthorization(options =>
    {
    var permissions = new[]
    {
        "products.view", "products.create", "products.edit", "products.delete",
        "invoices.view", "invoices.create", "invoices.approve",
        "reports.view", "users.manage", "accounts.manage"
    };

    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));
    }
    });

    var app = builder.Build();

    // Global Exception Middleware
    app.UseMiddleware<ExceptionMiddleware>();

    // authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Swagger
    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger();
    app.UseSwaggerUI();
    //}

    // Log every request
    app.UseSerilogRequestLogging();

    app.UseAuthorization();
    app.MapControllers();

    // Test endpoints
    app.MapGet("/hello", () =>
    {
        Log.Information("Hello endpoint was called");
        return "Hello from TaxAccount!";
    });

    Log.Information("TaxAccount API started successfully");
    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "TaxAccount API failed to start");
}
finally
{
    Log.CloseAndFlush();
}


// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };
// app.MapGet("/hello", () => "Hello from Docker!");
// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
