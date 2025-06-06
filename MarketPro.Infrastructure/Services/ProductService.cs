using MarketPro.Application.DTOs.Product;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace MarketPro.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Product> CreateProductAsync(CreateProductDto createProductDto)
        {
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

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            if (createProductDto.Images != null && createProductDto.Images.Any())
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadPath);

                foreach (var image in createProductDto.Images.Take(4)) // Максимум 4 изображения
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = $"/images/products/{fileName}",
                            IsPrimary = !product.Images.Any() // Первое изображение будет основным
                        };

                        _context.ProductImages.Add(productImage);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return product;
        }

        public async Task<IEnumerable<ProductType>> GetProductTypesAsync()
        {
            return await _context.ProductTypes.ToListAsync();
        }

        public async Task<IEnumerable<Store>> GetStoresAsync()
        {
            return await _context.Stores.ToListAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.Store)
                .Include(p => p.Images)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ShortDescription = p.ShortDescription,
                    Rating = (double)p.Rating,
                    ProductTypeName = p.ProductType.Name,
                    StoreName = p.Store.Name,
                    MainImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary).ImageUrl ?? p.Images.FirstOrDefault().ImageUrl
                })
                .ToListAsync();
        }
    }
} 