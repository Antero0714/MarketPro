using MarketPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using MarketPro.Infrastructure.Identity;

namespace MarketPro.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly IEmailSender _emailSender;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) //IEmailSender emailSender
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_emailSender = emailSender;
        }

        public async Task<bool> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return false;
                //return result.Errors(result.Errors.Select(e => e.Description).First());

            await _signInManager.SignInAsync(user, isPersistent: false);

            // Отправка письма
            // await _emailSender.SendAsync(email, "Регистрация", $"Ваш логин: {email}\nВаш пароль: {password}");

            return true;
            //return Result.Success();
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

            return result.Succeeded;
        }
    }
}
