using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Application.DTOs.Product;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;

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

            return View(new CreateProductDto { Rating = 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(25 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 25 * 1024 * 1024)]
        public async Task<IActionResult> Create(CreateProductDto createProductDto)
        {
            _logger.LogInformation("Starting Create POST action");

            // Проверяем наличие изображений
            if (createProductDto.Images == null || !createProductDto.Images.Any())
            {
                ModelState.AddModelError("Images", "Необходимо загрузить хотя бы одно изображение");
            }
            else if (createProductDto.Images.Count > 4)
            {
                ModelState.AddModelError("Images", "Можно загрузить не более 4 изображений");
            }
            else
            {
                // Проверяем размер файлов
                foreach (var image in createProductDto.Images)
                {
                    if (image.Length > 5 * 1024 * 1024) // 5MB
                    {
                        ModelState.AddModelError("Images", $"Файл {image.FileName} превышает допустимый размер (5MB)");
                    }
                }
            }

            // Логируем данные формы
            _logger.LogInformation($"Form data received: " +
                $"Name='{createProductDto.Name}', " +
                $"Price={createProductDto.Price}, " +
                $"ShortDescription='{createProductDto.ShortDescription}', " +
                $"DetailedDescription='{createProductDto.DetailedDescription}', " +
                $"Specifications='{createProductDto.Specifications}', " +
                $"Rating={createProductDto.Rating}, " +
                $"ProductTypeId={createProductDto.ProductTypeId}, " +
                $"StoreId={createProductDto.StoreId}, " +
                $"Images count={createProductDto.Images?.Count ?? 0}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Model Error: {modelError.ErrorMessage}");
                }

                ViewBag.ProductTypes = (await _productService.GetProductTypesAsync())?.ToList();
                ViewBag.Stores = (await _productService.GetStoresAsync())?.ToList();
                return View(createProductDto);
            }

            try
            {
                var product = await _productService.CreateProductAsync(createProductDto);
                _logger.LogInformation($"Product created successfully with ID: {product.Id}");

                TempData["Success"] = "Товар успешно создан";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                
                ModelState.AddModelError("", ex.Message);
                ViewBag.ProductTypes = (await _productService.GetProductTypesAsync())?.ToList();
                ViewBag.Stores = (await _productService.GetStoresAsync())?.ToList();
                return View(createProductDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Starting Edit GET action for product ID: {id}");

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {id} not found");
                TempData["Error"] = "Товар не найден";
                return RedirectToAction(nameof(Index));
            }

            var productTypes = await _productService.GetProductTypesAsync();
            var stores = await _productService.GetStoresAsync();

            ViewBag.ProductTypes = productTypes?.ToList();
            ViewBag.Stores = stores?.ToList();

            var editDto = new EditProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                DetailedDescription = product.DetailedDescription,
                Specifications = product.Specifications,
                Rating = product.Rating,
                ProductTypeId = product.ProductTypeId,
                StoreId = product.StoreId
            };

            return View(editDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(25 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 25 * 1024 * 1024)]
        public async Task<IActionResult> Edit(int id, EditProductDto editProductDto)
        {
            _logger.LogInformation($"Starting Edit POST action for product ID: {id}");

            if (id != editProductDto.Id)
            {
                _logger.LogWarning($"ID mismatch. URL ID: {id}, DTO ID: {editProductDto.Id}");
                return BadRequest();
            }

            if (editProductDto.Images != null && editProductDto.Images.Count > 4)
            {
                ModelState.AddModelError("Images", "Можно загрузить не более 4 изображений");
            }

            if (editProductDto.Images != null)
            {
                foreach (var image in editProductDto.Images)
                {
                    if (image.Length > 5 * 1024 * 1024) // 5MB
                    {
                        ModelState.AddModelError("Images", $"Файл {image.FileName} превышает допустимый размер (5MB)");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Model Error: {modelError.ErrorMessage}");
                }

                ViewBag.ProductTypes = (await _productService.GetProductTypesAsync())?.ToList();
                ViewBag.Stores = (await _productService.GetStoresAsync())?.ToList();
                return View(editProductDto);
            }

            try
            {
                await _productService.UpdateProductAsync(editProductDto);
                _logger.LogInformation($"Product updated successfully with ID: {id}");

                TempData["Success"] = "Товар успешно обновлен";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with ID: {id}");
                
                ModelState.AddModelError("", "Произошла ошибка при обновлении товара");
                ViewBag.ProductTypes = (await _productService.GetProductTypesAsync())?.ToList();
                ViewBag.Stores = (await _productService.GetStoresAsync())?.ToList();
                return View(editProductDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Starting Delete GET action for product ID: {id}");

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {id} not found");
                TempData["Error"] = "Товар не найден";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation($"Starting DeleteConfirmed action for product ID: {id}");

            try
            {
                await _productService.DeleteProductAsync(id);
                _logger.LogInformation($"Product deleted successfully with ID: {id}");

                TempData["Success"] = "Товар успешно удален";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID: {id}");
                TempData["Error"] = "Произошла ошибка при удалении товара";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 