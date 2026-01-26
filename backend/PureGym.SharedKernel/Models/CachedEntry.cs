namespace PureGym.SharedKernel.Models;

public sealed record CachedEntry<T>(T Data, DateTime ExpiresAt);
