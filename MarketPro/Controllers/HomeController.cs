using System.Diagnostics;
using System.Linq;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Models;
using MarketPro.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarketPro.Infrastructure.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using MarketPro.Application.DTOs.Product;

namespace MarketPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;

        public HomeController(
            ILogger<HomeController> logger,
            IProductService productService,
            ICartService cartService,
            IWishlistService wishlistService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
            _wishlistService = wishlistService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoryId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = userId != null ? _cartService.GetCartItems(userId).Select(x => x.ProductId).ToList() : new List<int>();
            var wishlistItems = userId != null ? _wishlistService.GetWishlistItems(userId).Select(x => x.ProductId).ToList() : new List<int>();

            // Get all products and categories
            var allProducts = await _productService.GetAllProductsAsync();
            var categories = await _productService.GetCategories();

            // Get phone category ID
            var phoneCategory = categories.FirstOrDefault(c => c.Name.ToLower().Contains("phone"));
            var phoneCategoryId = phoneCategory?.Id;

            // Get camera category ID
            var cameraCategory = categories.FirstOrDefault(c => c.Name.ToLower().Contains("camera"));
            var cameraCategoryId = cameraCategory?.Id;

            // Get headphone category ID
            var headphoneCategory = categories.FirstOrDefault(c => c.Name.ToLower().Contains("headphone"));
            var headphoneCategoryId = headphoneCategory?.Id;

            var viewModel = new HomeViewModel
            {
                SliderItems = new List<SliderItem>
                {
                    new SliderItem
                    {
                        Title = "Get The Sound You Love For Less",
                        ImageUrl = "/marketPro/assets/images/thumbs/cyber-monday-img1.png",
                        ButtonUrl = Url.Action("Index", "Catalog")
                    },
                    new SliderItem
                    {
                        Title = "Latest Smartphones at Best Prices",
                        ImageUrl = "/marketPro/assets/images/thumbs/cyber-monday-img2.png",
                        ButtonUrl = Url.Action("Index", "Catalog")
                    },
                    new SliderItem
                    {
                        Title = "Premium Electronics Collection",
                        ImageUrl = "/marketPro/assets/images/thumbs/cyber-monday-img3.png",
                        ButtonUrl = Url.Action("Index", "Catalog")
                    }
                },
                // Deal products (filtered by selected category if any)
                DealProducts = categoryId.HasValue 
                    ? allProducts.Where(p => p.ProductTypeId == categoryId).Take(8).ToList()
                    : allProducts.Take(8).ToList(),

                // Top Selling Products (headphones)
                TopSellingProducts = allProducts.Where(p => p.ProductTypeId == 2).Take(8).ToList(),

                // Trending Products (all products, filtered by category if selected)
                TrendingProducts =  allProducts.Where(p => p.ProductTypeId == 1).ToList(),

                // Featured Products (cameras)
                FeaturedProducts = allProducts.Where(p => p.ProductTypeId == 3).Take(8).ToList(),

                // Recommended Products (TVs)
                RecommendedProducts = allProducts.Where(p => p.ProductTypeId == 5).Take(8).ToList(),

                Categories = categories,
                SelectedCategoryId = categoryId,
                CartItemIds = cartItems,
                WishlistItemIds = wishlistItems
            };

            return View(viewModel);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
