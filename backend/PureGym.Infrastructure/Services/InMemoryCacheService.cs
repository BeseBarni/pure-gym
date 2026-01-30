using PureGym.Application.Interfaces.Services;
using PureGym.Infrastructure.Extensions;
using PureGym.SharedKernel.Models;
using System.Collections.Concurrent;

namespace PureGym.Infrastructure.Services;

internal class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, CachedEntry<object>> _cache = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public Task<CachedEntry<T>?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (!entry.IsExpired)
            {
                return Task.FromResult(entry?.GetCacheEntry<T>());
            }

            _cache.TryRemove(key, out _);
        }

        return Task.FromResult<CachedEntry<T>?>(default);
    }

    public Task<CachedEntry<T>> SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var entry = new CachedEntry<object>
        {
            Data = value,
            ExpiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null
        };

        _cache.AddOrUpdate(key, entry, (_, _) => entry);
        return Task.FromResult(entry.GetCacheEntry<T>());
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public async Task<CachedEntry<T>> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            cached = await GetAsync<T>(key, cancellationToken);
            if (cached != null)
            {
                return cached;
            }

            var value = await factory();
            var result = await SetAsync(key, value, expiration, cancellationToken);
            return result;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (!entry.IsExpired)
            {
                return Task.FromResult(true);
            }

            _cache.TryRemove(key, out _);
        }

        return Task.FromResult(false);
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        _cache.Clear();
        return Task.CompletedTask;
    }
}
