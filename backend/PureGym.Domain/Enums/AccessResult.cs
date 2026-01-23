namespace PureGym.Domain.Enums;

public enum AccessResult
{
    Granted = 0,
    DeniedExpired = 1,
    DeniedSuspended = 2,
    DeniedNoMembership = 3
}
