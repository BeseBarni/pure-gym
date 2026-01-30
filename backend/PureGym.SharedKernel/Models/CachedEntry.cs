namespace PureGym.SharedKernel.Models;

public sealed class CachedEntry<T>
{
    public T? Data { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public bool IsExpired => ExpiresAt.HasValue && DateTime.UtcNow >= ExpiresAt.Value;
}
