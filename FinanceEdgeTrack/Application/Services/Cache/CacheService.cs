using FinanceEdgeTrack.Domain.Interfaces.Services.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceEdgeTrack.Application.Services.Cache;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string SetCacheKey(string id)
            => $"CacheKey_user{id}";

    public Task SetAsync<T>(string cacheKey, T data, TimeSpan expiration)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = expiration,
            Priority = CacheItemPriority.Normal,
        };
        _cache.Set(cacheKey, data, cacheOptions);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string cacheKey)
    {
        _cache.Remove(cacheKey);
        return Task.CompletedTask;
    }

    public Task<T?> TryGetAsync<T>(string cacheKey)
    {
        _cache.TryGetValue(cacheKey, out T? data);
        return Task.FromResult(data);
    }
}
