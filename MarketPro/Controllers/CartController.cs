using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MarketPro.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetCartItems(userId);
            return View("~/Views/Cart/Index.cshtml", cartItems);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Please login to add items to cart" });
                }

                var result = await _cartService.AddToCart(userId, productId);
                var cartTotal = await _cartService.GetCartTotal(userId);
                return Json(new { success = true, cartTotal = cartTotal.ToString("C") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _cartService.RemoveFromCart(userId, productId);
                var cartTotal = await _cartService.GetCartTotal(userId);
                return Json(new { success = true, cartTotal = cartTotal.ToString("C") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _cartService.UpdateQuantity(userId, productId, quantity);
                var cartTotal = await _cartService.GetCartTotal(userId);
                return Json(new { success = true, cartTotal = cartTotal.ToString("C") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
} 