using Microsoft.AspNetCore.Mvc;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MarketPro.Models;
using System.Linq;

namespace MarketPro.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? categoryId = null)
        {
            _logger.LogInformation($"Запрошен список товаров{(categoryId.HasValue ? $" для категории {categoryId}" : "")}");

            var products = await _productService.GetProductsAsync(categoryId);
            var categories = await _productService.GetProductTypesAsync();

            var viewModel = new ProductListViewModel
            {
                Products = products.ToList(),
                Categories = categories.ToList(),
                SelectedCategoryId = categoryId
            };

            return View(viewModel);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Запрошены детали товара с ID: {id}");

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Товар с ID {id} не найден");
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Category/5
        public async Task<IActionResult> Category(int id)
        {
            _logger.LogInformation($"Запрошены товары категории с ID: {id}");

            var category = await _productService.GetProductTypeByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning($"Категория с ID {id} не найдена");
                return NotFound();
            }

            var products = await _productService.GetProductsAsync(id);
            var categories = await _productService.GetProductTypesAsync();

            var viewModel = new ProductListViewModel
            {
                Products = products.ToList(),
                Categories = categories.ToList(),
                SelectedCategoryId = id
            };

            return View("Index", viewModel);
        }
    }
} 