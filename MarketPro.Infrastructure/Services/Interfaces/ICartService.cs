using MarketPro.Domain.Entities;
using System.Collections.Generic;

namespace MarketPro.Infrastructure.Services.Interfaces
{
    public interface ICartService
    {
        IEnumerable<CartItem> GetCartItems(string userId);
        bool AddToCart(string userId, int productId);
        void RemoveFromCart(string userId, int productId);
        void UpdateQuantity(string userId, int productId, int quantity);
        decimal GetCartTotal(string userId);
    }
} 