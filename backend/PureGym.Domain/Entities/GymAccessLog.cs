using PureGym.Domain.Enums;

namespace PureGym.Domain.Entities;

public class GymAccessLog
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }
    public Guid? MembershipId { get; private set; }
    public DateTime AccessedAtUtc { get; private set; }
    public AccessResult Result { get; private set; }

    public Member Member { get; private set; } = null!;
    public Membership? Membership { get; private set; }

    private GymAccessLog() { }
    public static GymAccessLog Record(Member member, DateTime accessedAtUtc)
    {
        var activeMembership = member.IsDeleted ? null : member.GetActiveMembership();
        var result = DetermineAccessResult(member, activeMembership);

        return new GymAccessLog
        {
            Id = Guid.NewGuid(),
            MemberId = member.Id,
            MembershipId = activeMembership?.Id,
            AccessedAtUtc = accessedAtUtc,
            Result = result
        };
    }

    private static AccessResult DetermineAccessResult(Member member, Membership? membership)
    {
        if (member.IsDeleted)
            return AccessResult.DeniedNoMembership;

        if (membership is null)
            return AccessResult.DeniedNoMembership;

        if (membership.Status == MembershipStatus.Suspended)
            return AccessResult.DeniedSuspended;

        return membership.IsValid() ? AccessResult.Granted : AccessResult.DeniedExpired;
    }

    public bool WasGranted() => Result == AccessResult.Granted;
    public bool WasDenied() => Result != AccessResult.Granted;
}
