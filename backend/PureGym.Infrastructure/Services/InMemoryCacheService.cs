using PureGym.Application.Interfaces;
using System.Collections.Concurrent;

namespace PureGym.Infrastructure.Services;

internal class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (!entry.IsExpired)
            {
                return Task.FromResult((T?)entry.Value);
            }

            _cache.TryRemove(key, out _);
        }

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var entry = new CacheEntry
        {
            Value = value,
            ExpiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : null
        };

        _cache.AddOrUpdate(key, entry, (_, _) => entry);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
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
            await SetAsync(key, value, expiration, cancellationToken);
            return value;
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

    private class CacheEntry
    {
        public object? Value { get; init; }
        public DateTime? ExpiresAt { get; init; }
        public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value;
    }
}
