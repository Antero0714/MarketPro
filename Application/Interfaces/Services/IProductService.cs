using MarketPro.Application.DTOs.Product;
using MarketPro.Domain.Entities;

namespace MarketPro.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(CreateProductDto createProductDto);
        Task<IEnumerable<ProductType>> GetProductTypesAsync();
        Task<IEnumerable<Store>> GetStoresAsync();
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    }
} 