using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MarketPro.Infrastructure.Identity;
using MarketPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MarketPro.WebAPI.Areas.AdminPanel.Models;

namespace MarketPro.WebAPI.Areas.AdminPanel.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            await SetUserInfoAsync(_userManager);

            var totalUsers = await _userManager.Users.CountAsync();
            var totalWishlistItems = await _context.WishlistItems.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalCartItems = await _context.CartItems.CountAsync();

            var recentOrders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new OrderDetailsViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.OrderDate,
                    CustomerName = $"{o.FirstName} {o.LastName}",
                    Status = o.Status,
                    Items = o.OrderItems.Select(oi => new OrderItemViewModel
                    {
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        ProductId = oi.ProductId
                    }).ToList()
                })
                .ToListAsync();

            var dashboardViewModel = new DashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalWishlistItems = totalWishlistItems,
                TotalOrders = totalOrders,
                TotalCartItems = totalCartItems,
                RecentOrders = recentOrders
            };

            return View(dashboardViewModel);
        }

        // GET: DashboardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DashboardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DashboardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashboardController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashboardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
