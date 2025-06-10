using Microsoft.AspNetCore.Mvc;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

        // ... existing code ...
    }
} 