using Microsoft.EntityFrameworkCore;
using MarketPro.Application.DTOs.Product;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace MarketPro.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int MaxFileSize = 5 * 1024 * 1024; // 5MB
        private const int MaxImages = 4;

        public ProductService(
            ApplicationDbContext context, 
            IWebHostEnvironment webHostEnvironment,
            ILogger<ProductService> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<Product> CreateProductAsync(CreateProductDto createProductDto)
        {
            _logger.LogInformation($"Starting CreateProductAsync for product: {createProductDto.Name}");
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Проверяем существование ProductType и Store
                var productType = await _context.ProductTypes.FindAsync(createProductDto.ProductTypeId);
                if (productType == null)
                {
                    throw new InvalidOperationException($"ProductType with ID {createProductDto.ProductTypeId} not found");
                }

                var store = await _context.Stores.FindAsync(createProductDto.StoreId);
                if (store == null)
                {
                    throw new InvalidOperationException($"Store with ID {createProductDto.StoreId} not found");
                }

                _logger.LogInformation($"Creating product with ProductType: {productType.Name}, Store: {store.Name}");

                var product = new Product
                {
                    Name = createProductDto.Name,
                    Price = createProductDto.Price,
                    ShortDescription = createProductDto.ShortDescription,
                    DetailedDescription = createProductDto.DetailedDescription,
                    Specifications = createProductDto.Specifications,
                    Rating = createProductDto.Rating,
                    ProductTypeId = createProductDto.ProductTypeId,
                    StoreId = createProductDto.StoreId
                };

                _logger.LogInformation("Adding product to context");
                _context.Products.Add(product);
                
                var saveResult = await _context.SaveChangesAsync();
                _logger.LogInformation($"SaveChanges result for product: {saveResult}");

                if (createProductDto.Images != null && createProductDto.Images.Any())
                {
                    _logger.LogInformation($"Processing {createProductDto.Images.Count} images");
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    _logger.LogInformation($"Upload path: {uploadPath}");
                    
                    if (!Directory.Exists(uploadPath))
                    {
                        _logger.LogInformation("Creating upload directory");
                        Directory.CreateDirectory(uploadPath);
                    }

                    var processedImages = 0;
                    foreach (var image in createProductDto.Images.Take(MaxImages))
                    {
                        _logger.LogInformation($"Processing image: {image.FileName}, Size: {image.Length}");

                        if (image.Length == 0 || image.Length > MaxFileSize)
                        {
                            _logger.LogWarning($"Skipping image {image.FileName}: Invalid file size ({image.Length} bytes)");
                            continue;
                        }

                        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                        if (!_allowedExtensions.Contains(extension))
                        {
                            _logger.LogWarning($"Skipping image {image.FileName}: Invalid file extension ({extension})");
                            continue;
                        }

                        try
                        {
                            var fileName = $"{Guid.NewGuid()}{extension}";
                            var filePath = Path.Combine(uploadPath, fileName);
                            _logger.LogInformation($"Saving image to: {filePath}");

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            var productImage = new ProductImage
                            {
                                ProductId = product.Id,
                                ImageUrl = $"/images/products/{fileName}",
                                IsPrimary = processedImages == 0
                            };

                            _logger.LogInformation($"Adding image to database: {productImage.ImageUrl}, IsPrimary: {productImage.IsPrimary}");
                            _context.ProductImages.Add(productImage);
                            processedImages++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error saving image {image.FileName}");
                            throw; // Прерываем выполнение, чтобы транзакция откатилась
                        }
                    }

                    if (processedImages > 0)
                    {
                        var imagesSaveResult = await _context.SaveChangesAsync();
                        _logger.LogInformation($"SaveChanges result for images: {imagesSaveResult}");
                    }
                    else
                    {
                        throw new InvalidOperationException("No images were successfully processed");
                    }
                }
                else
                {
                    _logger.LogWarning("No images provided for product");
                    throw new InvalidOperationException("At least one image is required");
                }

                await transaction.CommitAsync();
                _logger.LogInformation($"Successfully created product with ID: {product.Id}");
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ProductTypeDto>> GetProductTypesAsync()
        {
            var productTypes = await _context.ProductTypes.ToListAsync();
            return productTypes.Select(pt => new ProductTypeDto
            {
                Id = pt.Id,
                Name = pt.Name
            });
        }

        public async Task<IEnumerable<Store>> GetStoresAsync()
        {
            return await _context.Stores.ToListAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(int? categoryId = null)
        {
            var query = _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.Store)
                .Include(p => p.Images)
                .Include(p => p.OrderItems)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.ProductTypeId == categoryId);
            }

            var products = await query.ToListAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.Store)
                .Include(p => p.Images)
                .Include(p => p.OrderItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            return MapToDto(product);
        }

        public async Task<Product> UpdateProductAsync(EditProductDto editProductDto)
        {
            _logger.LogInformation($"Starting UpdateProductAsync for product ID: {editProductDto.Id}");
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == editProductDto.Id);

                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {editProductDto.Id} not found");
                }

                // Проверяем существование ProductType и Store
                var productType = await _context.ProductTypes.FindAsync(editProductDto.ProductTypeId);
                if (productType == null)
                {
                    throw new InvalidOperationException($"ProductType with ID {editProductDto.ProductTypeId} not found");
                }

                var store = await _context.Stores.FindAsync(editProductDto.StoreId);
                if (store == null)
                {
                    throw new InvalidOperationException($"Store with ID {editProductDto.StoreId} not found");
                }

                // Обновляем основные данные продукта
                product.Name = editProductDto.Name;
                product.Price = editProductDto.Price;
                product.ShortDescription = editProductDto.ShortDescription;
                product.DetailedDescription = editProductDto.DetailedDescription;
                product.Specifications = editProductDto.Specifications;
                product.Rating = editProductDto.Rating;
                product.ProductTypeId = editProductDto.ProductTypeId;
                product.StoreId = editProductDto.StoreId;

                // Обрабатываем новые изображения, если они есть
                if (editProductDto.Images != null && editProductDto.Images.Any())
                {
                    _logger.LogInformation($"Processing {editProductDto.Images.Count} new images");
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                    // Удаляем старые изображения
                    foreach (var oldImage in product.Images)
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, oldImage.ImageUrl.TrimStart('/'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }
                    _context.ProductImages.RemoveRange(product.Images);

                    // Сохраняем новые изображения
                    var processedImages = 0;
                    foreach (var image in editProductDto.Images.Take(MaxImages))
                    {
                        if (image.Length == 0 || image.Length > MaxFileSize)
                        {
                            continue;
                        }

                        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                        if (!_allowedExtensions.Contains(extension))
                        {
                            continue;
                        }

                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = $"/images/products/{fileName}",
                            IsPrimary = processedImages == 0
                        };

                        _context.ProductImages.Add(productImage);
                        processedImages++;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Successfully updated product with ID: {product.Id}");
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product");
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            _logger.LogInformation($"Starting DeleteProductAsync for product ID: {id}");
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {id} not found");
                }

                // Удаляем файлы изображений
                foreach (var image in product.Images)
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                // Удаляем записи из базы данных
                _context.ProductImages.RemoveRange(product.Images);
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Successfully deleted product with ID: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product");
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                DetailedDescription = product.DetailedDescription,
                Specifications = product.Specifications,
                Rating = product.Rating,
                ProductTypeId = product.ProductTypeId,
                ProductTypeName = product.ProductType?.Name,
                StoreId = product.StoreId,
                StoreName = product.Store?.Name,
                ImageUrls = product.Images?.Select(i => i.ImageUrl),
                OrderCount = product.OrderItems?.Count ?? 0
            };
        }

        public async Task<ProductTypeDto> GetProductTypeByIdAsync(int id)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
                return null;

            return new ProductTypeDto
            {
                Id = productType.Id,
                Name = productType.Name
            };
        }

        public async Task<IEnumerable<ProductDto>> GetDealProducts(int? categoryId = null)
        {
            var query = _context.Products
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.Images)
                .Include(p => p.OrderItems)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.ProductTypeId == categoryId);
            }

            var products = await query
                .OrderByDescending(p => p.OrderItems.Count)
                .Take(10)
                .ToListAsync();

            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductTypeDto>> GetCategories()
        {
            var categories = await _context.ProductTypes
                .OrderBy(pt => pt.Name)
                .ToListAsync();

            return categories.Select(c => new ProductTypeDto
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.Store)
                .Include(p => p.Images)
                .Include(p => p.OrderItems)
                .ToListAsync();

            return products.Select(MapToDto);
        }
    }
}