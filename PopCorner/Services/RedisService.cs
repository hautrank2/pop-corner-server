using Microsoft.Extensions.Caching.Distributed;
using PopCorner.Models.Domains;
using PopCorner.Services.Interfaces;

namespace PopCorner.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _cache;

        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> SetDataAsync(string key, string value, DistributedCacheEntryOptions? options = null)
        {
            await _cache.SetStringAsync(key, value, options ?? new DistributedCacheEntryOptions { });
            return true;
        }

        public async Task<string?> GetDataAsync(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            return cachedData;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            var exists = await _cache.GetStringAsync(key) != null;
            if(!exists)
            {
                return false;
            }

            await _cache.RemoveAsync(key);
            return true;
        }
    }
}
