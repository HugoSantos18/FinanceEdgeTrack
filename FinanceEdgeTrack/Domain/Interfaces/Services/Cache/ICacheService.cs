namespace FinanceEdgeTrack.Domain.Interfaces.Services.Cache;

public interface ICacheService
{
    Task SetAsync<T>(string cacheKey, T data, TimeSpan expiration);
    Task<T?> TryGetAsync<T>(string cacheKey);
    Task RemoveAsync(string cacheKey);
    string SetCacheKey(Guid id); 

    // InvalidateCacheAfterChange(Guid id, T(specific)? data = null)
    // Pode ser utilizado e criado particularmente onde for utilizado o cache, não consegue pela interface pois
    // não da para tipar um generic com null.
}
