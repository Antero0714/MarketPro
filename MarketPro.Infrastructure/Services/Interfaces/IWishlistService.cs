using MarketPro.Domain.Entities;
using System.Collections.Generic;

namespace MarketPro.Infrastructure.Services.Interfaces
{
    public interface IWishlistService
    {
        IEnumerable<WishlistItem> GetWishlistItems(string userId);
        bool AddToWishlist(string userId, int productId);
        void RemoveFromWishlist(string userId, int productId);
    }
} 