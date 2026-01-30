using PureGym.SharedKernel.Models;

namespace PureGym.Application.Interfaces.Services;

public interface ICacheService
{
    Task<CachedEntry<T>?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    
    Task<CachedEntry<T>> SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
    
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    
    Task<CachedEntry<T>> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
    
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    
    Task ClearAsync(CancellationToken cancellationToken = default);
}
