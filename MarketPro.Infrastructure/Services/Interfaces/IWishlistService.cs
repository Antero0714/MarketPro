using MarketPro.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketPro.Infrastructure.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItem>> GetWishlistItems(string userId);
        Task<bool> AddToWishlist(string userId, int productId);
        Task RemoveFromWishlist(string userId, int productId);
    }
} 