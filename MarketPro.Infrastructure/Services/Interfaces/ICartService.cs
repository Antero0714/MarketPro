using MarketPro.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketPro.Infrastructure.Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> GetCartItems(string userId);
        Task<bool> AddToCart(string userId, int productId);
        Task RemoveFromCart(string userId, int productId);
        Task UpdateQuantity(string userId, int productId, int quantity);
        Task<decimal> GetCartTotal(string userId);
    }
} 