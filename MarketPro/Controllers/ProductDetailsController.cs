using Microsoft.AspNetCore.Mvc;
using MarketPro.Application.Interfaces.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MarketPro.Controllers
{
    public class ProductDetailsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductDetailsController> _logger;

        public ProductDetailsController(IProductService productService, ILogger<ProductDetailsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int id)
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
    }
} 