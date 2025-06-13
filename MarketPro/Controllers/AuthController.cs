using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MarketPro.Models.Auth;
using MarketPro.Models;
using System.Threading.Tasks;
using MarketPro.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MarketPro.Infrastructure.Data;

namespace MarketPro.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthController> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                try
                {
                    // Диагностика: проверяем всех пользователей в базе
                    var allUsers = await _context.Users.ToListAsync();
                    _logger.LogInformation($"Total users in database: {allUsers.Count}");
                    
                    // Обновляем нормализованные поля для всех пользователей
                    foreach (var dbUser in allUsers)
                    {
                        if (string.IsNullOrEmpty(dbUser.NormalizedEmail) || string.IsNullOrEmpty(dbUser.NormalizedUserName))
                        {
                            dbUser.NormalizedEmail = dbUser.Email?.ToUpper();
                            dbUser.NormalizedUserName = dbUser.UserName?.ToUpper();
                            await _userManager.UpdateAsync(dbUser);
                        }
                        _logger.LogInformation($"DB User: Email={dbUser.Email}, NormalizedEmail={dbUser.NormalizedEmail}, UserName={dbUser.UserName}");
                    }

                    // Ищем пользователя по email (не нормализованному)
                    var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == model.Email);

                    if (user == null)
                    {
                        _logger.LogWarning($"User not found with email: {model.Email}");
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(model);
                    }

                    _logger.LogInformation($"Found user: Id={user.Id}, Email={user.Email}, UserName={user.UserName}");

                    // Проверяем пароль
                    var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (!passwordValid)
                    {
                        _logger.LogWarning("Password validation failed");
                        ModelState.AddModelError(string.Empty, "Invalid password.");
                        return View(model);
                    }

                    _logger.LogInformation("Password validation succeeded");

                    // Пробуем войти
                    await _signInManager.SignOutAsync(); // Сначала выходим
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User {user.Email} logged in successfully");
                        
                        // Проверяем роль админа
                        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                        _logger.LogInformation($"User {user.Email} is admin: {isAdmin}");

                        if (isAdmin)
                        {
                            return RedirectToAction("Index", "Products", new { area = "AdminPanel" });
                        }
                        return RedirectToLocal(returnUrl);
                    }

                    _logger.LogWarning($"SignInManager.PasswordSignInAsync failed. Result: {result}");
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception during login: {ex}");
                    ModelState.AddModelError(string.Empty, "An error occurred during login.");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "User with this email already exists.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
