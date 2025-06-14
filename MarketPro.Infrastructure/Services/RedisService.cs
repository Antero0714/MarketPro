using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MarketPro.Infrastructure.Services
{
    public class RedisService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisService> _logger;
        private const string WishlistPrefix = "wishlist:";
        private const string CartPrefix = "cart:";

        public RedisService(IDistributedCache cache, ILogger<RedisService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions();
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration;
                }

                var jsonData = JsonSerializer.Serialize(value);
                await _cache.SetStringAsync(key, jsonData, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting Redis cache for key {Key}", key);
                // Не пробрасываем исключение, чтобы приложение могло работать без Redis
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var jsonData = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(jsonData))
                    return default;

                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Redis cache for key {Key}", key);
                return default;
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing Redis cache for key {Key}", key);
                // Не пробрасываем исключение, чтобы приложение могло работать без Redis
            }
        }

        public string GetWishlistKey(string userId) => $"{WishlistPrefix}{userId}";
        public string GetCartKey(string userId) => $"{CartPrefix}{userId}";
    }
} 