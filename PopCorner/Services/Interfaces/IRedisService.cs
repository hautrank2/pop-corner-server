using Microsoft.Extensions.Caching.Distributed;

namespace PopCorner.Services.Interfaces
{
    public interface IRedisService
    {
        Task<string?> GetDataAsync(string key);
        Task<bool> SetDataAsync(string key, string value, DistributedCacheEntryOptions? options = null);
        Task<bool> RemoveAsync(string key);
    }
}
