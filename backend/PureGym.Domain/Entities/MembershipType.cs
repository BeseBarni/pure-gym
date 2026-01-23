using PureGym.Domain.Exceptions;
using PureGym.Domain.Interfaces;

namespace PureGym.Domain.Entities;

public class MembershipType : ISoftDeletable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal PricePerMonth { get; private set; }
    public int DurationInDays { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public ICollection<Membership> Memberships { get; private set; } = [];

    private MembershipType() { }

    public static MembershipType Create(string name, decimal pricePerMonth, int durationInDays, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.Required(nameof(name));

        if (pricePerMonth < 0)
            throw DomainException.InvalidPrice(pricePerMonth);

        if (durationInDays <= 0)
            throw DomainException.InvalidDuration(durationInDays);

        return new MembershipType
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            PricePerMonth = pricePerMonth,
            DurationInDays = durationInDays,
            IsActive = true
        };
    }

    public bool CanBeUsedForNewMembership() => IsActive && !IsDeleted;

    public void UpdateDetails(string name, string? description, decimal pricePerMonth, int durationInDays)
    {
        ThrowIfDeleted();

        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.Required(nameof(name));

        if (pricePerMonth < 0)
            throw DomainException.InvalidPrice(pricePerMonth);

        if (durationInDays <= 0)
            throw DomainException.InvalidDuration(durationInDays);

        Name = name.Trim();
        Description = description?.Trim();
        PricePerMonth = pricePerMonth;
        DurationInDays = durationInDays;
    }

    public void Deactivate()
    {
        ThrowIfDeleted();
        IsActive = false;
    }

    public void Activate()
    {
        ThrowIfDeleted();
        IsActive = true;
    }

    public void SoftDelete()
    {
        if (IsDeleted)
            throw DomainException.AlreadyDeleted(nameof(MembershipType), Id);

        IsActive = false;
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw DomainException.NotDeleted(nameof(MembershipType), Id);

        IsDeleted = false;
        DeletedAtUtc = null;
    }

    private void ThrowIfDeleted()
    {
        if (IsDeleted)
            throw DomainException.EntityDeleted(nameof(MembershipType), Id);
    }
}
