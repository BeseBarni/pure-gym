using PureGym.Domain.Enums;
using PureGym.Domain.Exceptions;

namespace PureGym.Domain.Entities;

public class Member : BaseSoftDeletableEntity
{
    public override Guid Id { get; protected set; } = Guid.NewGuid();
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    protected override string EntityName => nameof(Member);

    public ICollection<Membership> Memberships { get; private set; } = [];
    public ICollection<GymAccessLog> AccessLogs { get; private set; } = [];
    public ICollection<MemberOrder> Orders { get; private set; } = [];

    public string FullName => $"{FirstName} {LastName}";

    private Member() { }

    public static Member Create(string firstName, string lastName, string email, DateTime dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw DomainException.Required(nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw DomainException.Required(nameof(lastName));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw DomainException.InvalidEmail(email ?? string.Empty);

        return new Member
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.ToLowerInvariant().Trim(),
            DateOfBirth = dateOfBirth,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void LinkToUser(Guid userId)
    {
        Id = userId;
    }

    public Membership? GetActiveMembership()
    {
        ThrowIfDeleted();
        return Memberships.FirstOrDefault(m => m.IsValid());
    }

    public bool CanEnterGym()
    {
        if (IsDeleted) return false;
        return GetActiveMembership() is not null;
    }

    public void AddMembership(Membership membership)
    {
        ThrowIfDeleted();
        Memberships.Add(membership);
    }

    public void UpdateContactInfo(string? phoneNumber, string? email = null)
    {
        ThrowIfDeleted();
        PhoneNumber = phoneNumber?.Trim();

        if (email is not null)
        {
            if (!email.Contains('@'))
                throw DomainException.InvalidEmail(email);

            Email = email.ToLowerInvariant().Trim();
        }
    }

    public void UpdateName(string firstName, string lastName)
    {
        ThrowIfDeleted();

        if (string.IsNullOrWhiteSpace(firstName))
            throw DomainException.Required(nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw DomainException.Required(nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    protected override void OnBeforeSoftDelete()
    {
        foreach (var membership in Memberships.Where(m => m.IsValid()))
        {
            membership.Cancel();
        }
    }

    public bool HasActiveOrPendingMembership()
    {
        if (IsDeleted) return false;

        return Memberships.Any(m =>
            !m.IsDeleted &&
            (m.Status == MembershipStatus.Active || m.Status == MembershipStatus.Pending) &&
            !m.IsExpired()
        );
    }
}
