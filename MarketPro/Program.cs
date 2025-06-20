using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketPro.Infrastructure.Data;
using MarketPro.Infrastructure.Identity;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using MarketPro.Models.ViewModels;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Infrastructure.Services;
using Microsoft.AspNetCore.Http.Features;
using MarketPro.Infrastructure.Services.Interfaces;
using StackExchange.Redis;
using System.Security.Authentication;
using MarketPro.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configure form options for file uploads
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 25 * 1024 * 1024; 
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("MarketPro.Infrastructure")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    
    // Отключаем требования к подтверждению email
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Register application services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<RedisService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("ConnectionStrings:Redis").Value;
    options.InstanceName = "MarketPro_";
});

// Configure cookie policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

// Add SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

app.UseExceptionHandler("/Error"); // Обрабатывает исключения (например, 500)
app.UseStatusCodePagesWithReExecute("/Error/{0}"); // Обрабатывает статус-коды (например, 404)

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hub
app.MapHub<OrderHub>("/orderHub");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "auth",
    pattern: "auth/{action=Login}/{id?}",
    defaults: new { controller = "Auth" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        // Ensure database is created and migrations are applied
        context.Database.Migrate();

        // Create roles if they don't exist
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                logger.LogInformation($"Created role {roleName}");
            }
        }

        // Create admin user if it doesn't exist
        var adminEmail = "andrey@mail.ru";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                NormalizedUserName = adminEmail.ToUpper(),
                NormalizedEmail = adminEmail.ToUpper(),
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var password = "Andrey_1407"; // Используем ваш пароль
            var result = await userManager.CreateAsync(adminUser, password);

            if (result.Succeeded)
            {
                logger.LogInformation("Admin user created successfully");
            }
            else
            {
                logger.LogError("Failed to create admin user");
                foreach (var error in result.Errors)
                {
                    logger.LogError($"Error: {error.Description}");
                }
            }
        }
        else
        {
            // Обновляем существующие поля
            adminUser.UserName = adminEmail;
            adminUser.NormalizedUserName = adminEmail.ToUpper();
            adminUser.Email = adminEmail;
            adminUser.NormalizedEmail = adminEmail.ToUpper();
            adminUser.EmailConfirmed = true;
            await userManager.UpdateAsync(adminUser);

            // Сбрасываем пароль
            var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            var result = await userManager.ResetPasswordAsync(adminUser, token, "Andrey_1407");
            if (result.Succeeded)
            {
                logger.LogInformation("Password reset to Andrey_1407 for andrey@mail.ru");
            }
            else
            {
                logger.LogError("Failed to reset password");
                foreach (var error in result.Errors)
                {
                    logger.LogError($"Error: {error.Description}");
                }
            }
        }

        // Ensure admin user is in Admin role
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
            if (addToRoleResult.Succeeded)
            {
                logger.LogInformation($"Added user {adminEmail} to Admin role");
            }
            else
            {
                logger.LogError($"Failed to add user {adminEmail} to Admin role");
                foreach (var error in addToRoleResult.Errors)
                {
                    logger.LogError($"Error: {error.Description}");
                }   
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();