using PureGym.SharedKernel.Models;

namespace PureGym.Infrastructure.Extensions;

public static class CacheExtensions
{
    public static CachedEntry<T> GetCacheEntry<T>(this CachedEntry<object> entry) where T : class
    {
        return new CachedEntry<T>
        {
            Data = entry.Data as T,
            ExpiresAt = entry.ExpiresAt
        };
    }
}
