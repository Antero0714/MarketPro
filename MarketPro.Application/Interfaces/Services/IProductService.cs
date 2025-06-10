using MarketPro.Domain.Entities;
using System.Threading.Tasks;

namespace MarketPro.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
    }
} 