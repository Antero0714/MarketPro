using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MarketPro.WebAPI.Areas.AdminPanel.Controllers
{
    public class ProductsCatalogController : AdminBaseController
    {
        private readonly IProductService _productService;

        public ProductsCatalogController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: ProductsCatalogController
        public async Task<IActionResult> Index(int? categoryId = null, int page = 1, decimal? minPrice = null, decimal? maxPrice = null, int? rating = null, string brand = null)
        {
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
            const int pageSize = 12; // Show more items per page in admin panel
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
                CurrentPage = page,
                TotalPages = totalPages,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Rating = rating,
                Brand = brand
            };

            return View(viewModel);
        }

        // GET: ProductsCatalogController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: ProductsCatalogController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductsCatalogController/Create
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

        // GET: ProductsCatalogController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductsCatalogController/Edit/5
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

        // GET: ProductsCatalogController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductsCatalogController/Delete/5
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
