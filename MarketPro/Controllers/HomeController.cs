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
        public async Task<IActionResult> Index(int? categoryId = null, int? featuredCount = 8, int? dealsCount = 8, int? recommendedCount = 8)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = userId != null ? _cartService.GetCartItems(userId).Select(x => x.ProductId).ToList() : new List<int>();
            var wishlistItems = userId != null ? _wishlistService.GetWishlistItems(userId).Select(x => x.ProductId).ToList() : new List<int>();

            // Get deal products and categories using the new methods
            var dealProducts = await _productService.GetDealProducts(categoryId);
            var categories = await _productService.GetCategories();

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
                DealProducts = dealProducts,
                Categories = categories,
                SelectedCategoryId = categoryId,
                CartItemIds = cartItems,
                WishlistItemIds = wishlistItems
            };

            // Get all products
            var allProducts = await _productService.GetAllProductsAsync();
            
            // Featured products (highest rated)
            viewModel.FeaturedProducts = allProducts
                .OrderByDescending(p => p.Rating)
                .Take(featuredCount ?? 8)
                .ToList();

            // Recommended products (mix of rating and orders)
            viewModel.RecommendedProducts = allProducts
                .OrderByDescending(p => ((double)p.Rating * 0.7) + (p.OrderCount * 0.3))
                .Take(recommendedCount ?? 8)
                .ToList();

            // If category filter is applied, filter all product lists
            if (categoryId.HasValue)
            {
                viewModel.FeaturedProducts = viewModel.FeaturedProducts
                    .Where(p => p.ProductTypeId == categoryId)
                    .ToList();
                    
                viewModel.RecommendedProducts = viewModel.RecommendedProducts
                    .Where(p => p.ProductTypeId == categoryId)
                    .ToList();
            }

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
