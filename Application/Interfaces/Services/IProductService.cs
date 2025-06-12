using MarketPro.Application.DTOs;
using MarketPro.Application.DTOs.Product;
using MarketPro.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketPro.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetProductsAsync(int? categoryId = null);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductTypeDto>> GetProductTypesAsync();
        Task<ProductTypeDto> GetProductTypeByIdAsync(int id);
        Task<IEnumerable<Store>> GetStoresAsync();
        Task<Product> CreateProductAsync(CreateProductDto createProductDto);
        Task<Product> UpdateProductAsync(EditProductDto editProductDto);
        Task DeleteProductAsync(int id);
        Task<IEnumerable<ProductDto>> GetDealProducts(int? categoryId = null);
        Task<IEnumerable<ProductTypeDto>> GetCategories();
    }
} 