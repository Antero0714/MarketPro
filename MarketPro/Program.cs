using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarketPro.Infrastructure.Data;
using MarketPro.Infrastructure.Identity;
using MarketPro.Models.ViewModels;

var builder = WebApplication.CreateBuilder(args);

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

// Configure cookie policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

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
                await userManager.AddToRoleAsync(adminUser, "Admin");
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

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
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