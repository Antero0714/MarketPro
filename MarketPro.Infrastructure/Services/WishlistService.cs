using MarketPro.Domain.Entities;
using MarketPro.Infrastructure.Data;
using MarketPro.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketPro.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;
        private readonly RedisService _redisService;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(24);

        public WishlistService(ApplicationDbContext context, RedisService redisService)
        {
            _context = context;
            _redisService = redisService;
        }

        public async Task<IEnumerable<WishlistItem>> GetWishlistItems(string userId)
        {
            // Try to get from cache first
            var cacheKey = _redisService.GetWishlistKey(userId);
            var cachedItems = await _redisService.GetAsync<List<WishlistItem>>(cacheKey);
            
            if (cachedItems != null)
            {
                return cachedItems;
            }

            // If not in cache, get from database
            var items = await _context.WishlistItems
                .Include(w => w.Product)
                    .ThenInclude(p => p.Images)
                .Where(w => w.UserId == userId)
                .ToListAsync();

            // Cache the result
            await _redisService.SetAsync(cacheKey, items, _cacheExpiration);

            return items;
        }

        public async Task<bool> AddToWishlist(string userId, int productId)
        {
            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingItem != null)
            {
                return false;
            }

            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = productId,
                AddedDate = DateTime.UtcNow
            };

            _context.WishlistItems.Add(wishlistItem);
            await _context.SaveChangesAsync();

            // Invalidate cache
            var cacheKey = _redisService.GetWishlistKey(userId);
            await _redisService.RemoveAsync(cacheKey);

            return true;
        }

        public async Task RemoveFromWishlist(string userId, int productId)
        {
            var wishlistItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem != null)
            {
                _context.WishlistItems.Remove(wishlistItem);
                await _context.SaveChangesAsync();

                // Invalidate cache
                var cacheKey = _redisService.GetWishlistKey(userId);
                await _redisService.RemoveAsync(cacheKey);
            }
        }
    }
} 