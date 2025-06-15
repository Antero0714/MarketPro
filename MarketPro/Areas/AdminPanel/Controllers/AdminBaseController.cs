using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MarketPro.Infrastructure.Identity;
using System.Security.Claims;

namespace MarketPro.WebAPI.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public abstract class AdminBaseController : Controller
    {
        protected async Task SetUserInfoAsync(UserManager<ApplicationUser> userManager)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    ViewData["UserFullName"] = $"{user.FirstName} {user.LastName}".Trim();
                    ViewData["UserEmail"] = user.Email;
                }
            }
        }
    }
} 