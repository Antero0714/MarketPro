using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using MarketPro.Infrastructure.Services.Interfaces;

namespace MarketPro.WebAPI.Controllers
{
    public class Catalog : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;

        public Catalog(
            IProductService productService,
            ICartService cartService,
            IWishlistService wishlistService)
        {
            _productService = productService;
            _cartService = cartService;
            _wishlistService = wishlistService;
        }

        // GET: Catalog
        public async Task<IActionResult> Index(int? categoryId = null, int page = 1, decimal? minPrice = null, decimal? maxPrice = null, int? rating = null, string brand = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = userId != null
                ? (await _cartService.GetCartItems(userId)).Select(x => x.ProductId).ToList()
                : new List<int>();

            var wishlistItems = userId != null
                ? (await _wishlistService.GetWishlistItems(userId)).Select(x => x.ProductId).ToList()
                : new List<int>();

            // Get all products and categories
            var allProducts = await _productService.GetAllProductsAsync();
            var categories = await _productService.GetCategories();

            // Apply filters
            var filteredProducts = allProducts;

            if (categoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.ProductTypeId == categoryId);
            }

            if (minPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= maxPrice.Value);
            }

            if (rating.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Rating >= rating.Value);
            }

            if (!string.IsNullOrEmpty(brand))
            {
                filteredProducts = filteredProducts.Where(p => p.StoreName.ToLower() == brand.ToLower());
            }

            // Calculate pagination
            const int pageSize = 4;
            var totalItems = filteredProducts.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            // Get paginated products
            var paginatedProducts = filteredProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var viewModel = new CatalogViewModel
            {
                Products = paginatedProducts.ToList(),
                Categories = categories.ToList(),
                SelectedCategoryId = categoryId,
                CartItemIds = cartItems,
                WishlistItemIds = wishlistItems,
                CurrentPage = page,
                TotalPages = totalPages,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Rating = rating,
                Brand = brand
            };

            return View(viewModel);
        }

        // GET: Catalog/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Catalog/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Catalog/Create
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

        // GET: Catalog/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Catalog/Edit/5
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

        // GET: Catalog/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Catalog/Delete/5
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
