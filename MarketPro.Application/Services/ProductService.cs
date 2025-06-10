using Microsoft.EntityFrameworkCore;
using MarketPro.Application.Interfaces.Services;
using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Data;
using System.Threading.Tasks;

namespace MarketPro.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Store)
                .Include(p => p.ProductType)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // ... existing code ...
    }
} 