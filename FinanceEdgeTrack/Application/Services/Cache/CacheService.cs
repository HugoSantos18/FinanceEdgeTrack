using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services.Cache;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata.Ecma335;

namespace FinanceEdgeTrack.Application.Services.Cache;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheService> _logger;
    private readonly IUnitOfWork _uof;

    public CacheService(IMemoryCache cache, ILogger<CacheService> logger, IUnitOfWork uof)
    {
        _cache = cache;
        _logger = logger;
        _uof = uof;
    }
    public string SetCacheKey(Guid id)
            => $"CacheKey_user{id}";

    public Task SetAsync<T>(string cacheKey, T data, TimeSpan expiration)
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(4),
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
