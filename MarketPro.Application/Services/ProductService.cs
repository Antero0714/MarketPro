using Microsoft.EntityFrameworkCore;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Application.DTOs.Product;
using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Data;
using System.Threading.Tasks;
using System.Linq;

namespace MarketPro.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                DetailedDescription = product.DetailedDescription,
                Rating = product.Rating,
                ProductTypeId = product.ProductTypeId,
                ProductTypeName = product.ProductType.Name,
                StoreName = product.Store.Name,
                Images = product.Images.Select(i => new ProductImageDto 
                {
                    ImageUrl = i.ImageUrl,
                    IsPrimary = i.IsPrimary
                }).ToList(),
                Specifications = product.Specifications
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

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ShortDescription = p.ShortDescription,
                DetailedDescription = p.DetailedDescription,
                Rating = p.Rating,
                ProductTypeId = p.ProductTypeId,
                ProductTypeName = p.ProductType.Name,
                StoreName = p.Store.Name,
                Images = p.Images.Select(i => new ProductImageDto 
                {
                    ImageUrl = i.ImageUrl,
                    IsPrimary = i.IsPrimary
                }).ToList(),
                Specifications = p.Specifications,
                OrderItemsCount = p.OrderItems.Count
            });
        }

        public async Task<IEnumerable<ProductTypeDto>> GetCategories()
        {
            var categories = await _context.ProductTypes
                .OrderBy(pt => pt.Name)
                .ToListAsync();

            return categories.Select(c => new ProductTypeDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }
    }
} 