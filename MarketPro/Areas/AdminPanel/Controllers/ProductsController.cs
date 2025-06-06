using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Application.DTOs.Product;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace MarketPro.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Starting Create action");
            
            var productTypes = await _productService.GetProductTypesAsync();
            var stores = await _productService.GetStoresAsync();

            _logger.LogInformation($"ProductTypes count: {productTypes?.Count() ?? 0}");
            _logger.LogInformation($"Stores count: {stores?.Count() ?? 0}");
            _logger.LogInformation($"ProductTypes type: {productTypes?.GetType().FullName ?? "null"}");
            _logger.LogInformation($"Stores type: {stores?.GetType().FullName ?? "null"}");

            if (productTypes != null)
            {
                foreach (var type in productTypes)
                {
                    _logger.LogInformation($"ProductType: Id={type.Id}, Name={type.Name}");
                }
            }
            else
            {
                _logger.LogWarning("ProductTypes is null");
            }

            if (stores != null)
            {
                foreach (var store in stores)
                {
                    _logger.LogInformation($"Store: Id={store.Id}, Name={store.Name}");
                }
            }
            else
            {
                _logger.LogWarning("Stores is null");
            }

            ViewBag.ProductTypes = productTypes?.ToList();
            ViewBag.Stores = stores?.ToList();

            _logger.LogInformation($"ViewBag.ProductTypes is null: {ViewBag.ProductTypes == null}");
            _logger.LogInformation($"ViewBag.Stores is null: {ViewBag.Stores == null}");
            _logger.LogInformation($"ViewBag.ProductTypes type: {ViewBag.ProductTypes?.GetType().FullName ?? "null"}");
            _logger.LogInformation($"ViewBag.Stores type: {ViewBag.Stores?.GetType().FullName ?? "null"}");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProductTypes = (await _productService.GetProductTypesAsync())?.ToList();
                ViewBag.Stores = (await _productService.GetStoresAsync())?.ToList();
                return View(createProductDto);
            }

            try
            {
                var product = await _productService.CreateProductAsync(createProductDto);
                TempData["Success"] = "Товар успешно создан";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                ModelState.AddModelError("", "Произошла ошибка при создании товара");
                ViewBag.ProductTypes = (await _productService.GetProductTypesAsync())?.ToList();
                ViewBag.Stores = (await _productService.GetStoresAsync())?.ToList();
                return View(createProductDto);
            }
        }
    }
} 