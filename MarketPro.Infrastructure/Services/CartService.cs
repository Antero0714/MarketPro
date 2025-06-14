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
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly RedisService _redisService;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(24);

        public CartService(ApplicationDbContext context, RedisService redisService)
        {
            _context = context;
            _redisService = redisService;
        }

        public async Task<IEnumerable<CartItem>> GetCartItems(string userId)
        {
            // Try to get from cache first
            var cacheKey = _redisService.GetCartKey(userId);
            var cachedItems = await _redisService.GetAsync<List<CartItem>>(cacheKey);
            
            if (cachedItems != null)
            {
                return cachedItems;
            }

            // If not in cache, get from database
            var items = await _context.CartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Images)
                .Include(c => c.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(c => c.Product)
                    .ThenInclude(p => p.Store)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            // Cache the result
            await _redisService.SetAsync(cacheKey, items, _cacheExpiration);

            return items;
        }

        public async Task<bool> AddToCart(string userId, int productId)
        {
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1,
                    AddedDate = DateTime.UtcNow
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            // Invalidate cache
            var cacheKey = _redisService.GetCartKey(userId);
            await _redisService.RemoveAsync(cacheKey);

            return true;
        }

        public async Task RemoveFromCart(string userId, int productId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                // Invalidate cache
                var cacheKey = _redisService.GetCartKey(userId);
                await _redisService.RemoveAsync(cacheKey);
            }
        }

        public async Task UpdateQuantity(string userId, int productId, int quantity)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();

                // Invalidate cache
                var cacheKey = _redisService.GetCartKey(userId);
                await _redisService.RemoveAsync(cacheKey);
            }
        }

        public async Task<decimal> GetCartTotal(string userId)
        {
            var cartItems = await GetCartItems(userId);
            return cartItems.Sum(c => c.Product.Price * c.Quantity);
        }
    }
} 