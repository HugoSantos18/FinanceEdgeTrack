namespace FinanceEdgeTrack.Application.Interfaces.Services.Cache;

public interface ICacheService
{
    Task SetAsync<T>(string cacheKey, T data, TimeSpan expiration);
    Task<T?> TryGetAsync<T>(string cacheKey);
    Task RemoveAsync(string cacheKey);
    string SetCacheKey(string id); 
}
