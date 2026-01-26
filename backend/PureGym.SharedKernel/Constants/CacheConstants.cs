namespace PureGym.SharedKernel.Constants;

public static class CacheKeys
{
    public static string MemberAccess(Guid memberId) => $"access:member:{memberId}";
}
