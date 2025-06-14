using Microsoft.AspNetCore.Mvc;
using MarketPro.Infrastructure.Services;
using MarketPro.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MarketPro.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class RedisDebugController : ControllerBase
    {
        private readonly RedisService _redisService;

        public RedisDebugController(RedisService redisService)
        {
            _redisService = redisService;
        }

        [HttpGet("wishlist/{userId}")]
        public async Task<ActionResult<IEnumerable<WishlistItem>>> GetWishlistData(string userId)
        {
            var cacheKey = _redisService.GetWishlistKey(userId);
            var data = await _redisService.GetAsync<List<WishlistItem>>(cacheKey);
            
            if (data == null)
                return NotFound($"No cached wishlist data found for user {userId}");

            return Ok(new
            {
                CacheKey = cacheKey,
                ItemsCount = data.Count,
                Items = data
            });
        }

        [HttpGet("cart/{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartData(string userId)
        {
            var cacheKey = _redisService.GetCartKey(userId);
            var data = await _redisService.GetAsync<List<CartItem>>(cacheKey);
            
            if (data == null)
                return NotFound($"No cached cart data found for user {userId}");

            return Ok(new
            {
                CacheKey = cacheKey,
                ItemsCount = data.Count,
                TotalItems = data.Sum(x => x.Quantity),
                Items = data
            });
        }

        [HttpDelete("wishlist/{userId}")]
        public async Task<IActionResult> ClearWishlistCache(string userId)
        {
            var cacheKey = _redisService.GetWishlistKey(userId);
            await _redisService.RemoveAsync(cacheKey);
            return Ok($"Cleared wishlist cache for user {userId}");
        }

        [HttpDelete("cart/{userId}")]
        public async Task<IActionResult> ClearCartCache(string userId)
        {
            var cacheKey = _redisService.GetCartKey(userId);
            await _redisService.RemoveAsync(cacheKey);
            return Ok($"Cleared cart cache for user {userId}");
        }
    }
} 