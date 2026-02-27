using PureGym.Domain.Enums;
using PureGym.Domain.Exceptions;

namespace PureGym.Domain.Entities;

public class Membership : BaseSoftDeletableEntity
{
    public override Guid Id { get; protected set; } = Guid.NewGuid();
    public Guid MemberId { get; private set; }
    public Guid MembershipTypeId { get; private set; }
    public DateTime StartDateUtc { get; private set; }
    public DateTime EndDateUtc { get; private set; }
    public MembershipStatus Status { get; private set; }
    public DateTime? PendingSinceUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    protected override string EntityName => nameof(Membership);

    public Member Member { get; private set; } = null!;
    public MembershipType MembershipType { get; private set; } = null!;
    public ICollection<GymAccessLog> AccessLogs { get; private set; } = [];
    public MemberOrder? Orders { get; private set; }

    public int DaysRemaining => IsValid() ? (EndDateUtc - DateTime.UtcNow).Days : 0;

    private Membership() { }

    public static Membership Create(Member member, MembershipType type)
    {
        if (member.IsDeleted)
            throw DomainException.EntityDeleted(nameof(Member), member.Id);

        if (type.IsDeleted)
            throw DomainException.EntityDeleted(nameof(MembershipType), type.Id);

        if (!type.IsActive)
            throw DomainException.MembershipTypeInactive(type.Id);

        var now = DateTime.UtcNow;
        var membership = new Membership
        {
            MemberId = member.Id,
            MembershipTypeId = type.Id,
            StartDateUtc = now,
            EndDateUtc = now.AddDays(type.DurationInDays),
            Status = MembershipStatus.Active,
            CreatedAtUtc = now
        };

        member.AddMembership(membership);
        return membership;
    }
    public static Membership CreatePending(Member member, MembershipType type) 
    {
        if (member.IsDeleted)
            throw DomainException.EntityDeleted(nameof(Member), member.Id);
        if (type.IsDeleted)
            throw DomainException.EntityDeleted(nameof(MembershipType), type.Id);
        if (!type.IsActive)
            throw DomainException.MembershipTypeInactive(type.Id);

        var now = DateTime.UtcNow;
        var expiry = now.Date.AddDays(type.DurationInDays).AddDays(-1).AddSeconds(-1);
        var membership = new Membership
        {
            MemberId = member.Id,
            MembershipTypeId = type.Id,
            StartDateUtc = now,
            EndDateUtc = now.AddDays(type.DurationInDays),
            Status = MembershipStatus.Pending,
            PendingSinceUtc = now,
            CreatedAtUtc = now
        };

        member.AddMembership(membership);
        return membership;
    }

    public bool IsPending() => Status == MembershipStatus.Pending;
    public bool IsValid() =>
        !IsDeleted &&
        Status == MembershipStatus.Active &&
        DateTime.UtcNow >= StartDateUtc &&
        DateTime.UtcNow <= EndDateUtc;

    public bool IsExpired() => DateTime.UtcNow > EndDateUtc;

    public void Activate()
    {
        ThrowIfDeleted();

        if (IsExpired())
            throw DomainException.MembershipExpired(Id);

        if (Status != MembershipStatus.Pending)
            throw DomainException.InvalidState(nameof(Membership), Status.ToString(), "activate");

        Status = MembershipStatus.Active;
        PendingSinceUtc = null;
    }

    public void Cancel()
    {
        ThrowIfDeleted();

        if (Status == MembershipStatus.Cancelled)
            throw DomainException.MembershipAlreadyCancelled(Id);

        Status = MembershipStatus.Cancelled;
    }

    public void Suspend()
    {
        ThrowIfDeleted();

        if (Status != MembershipStatus.Active)
            throw DomainException.InvalidState(nameof(Membership), Status.ToString(), "suspend");

        Status = MembershipStatus.Suspended;
    }

    public void Reactivate()
    {
        ThrowIfDeleted();

        if (IsExpired())
            throw DomainException.MembershipExpired(Id);

        if (Status == MembershipStatus.Active)
            throw DomainException.InvalidState(nameof(Membership), nameof(MembershipStatus.Active), "reactivate");

        Status = MembershipStatus.Active;
    }

    public void Extend(int additionalDays)
    {
        ThrowIfDeleted();

        if (additionalDays <= 0)
            throw DomainException.InvalidDuration(additionalDays);

        EndDateUtc = EndDateUtc.AddDays(additionalDays);
    }

}
