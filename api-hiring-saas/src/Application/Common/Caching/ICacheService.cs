namespace Application.Common.Caching;

public interface ICacheService
{
    Task RemoveAsync(string key, CancellationToken cancellationToken);
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);
    Task AddAsync<T>(string key, T value, CancellationToken cancellationToken);
    Task ClearCache(CancellationToken cancellationToken);
}
