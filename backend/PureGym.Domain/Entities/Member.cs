using PureGym.Domain.Enums;
using PureGym.Domain.Exceptions;
using PureGym.Domain.Interfaces;

namespace PureGym.Domain.Entities;

public class Member : ISoftDeletable
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid? UserId { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public ICollection<Membership> Memberships { get; private set; } = [];
    public ICollection<GymAccessLog> AccessLogs { get; private set; } = [];

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
            Id = Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.ToLowerInvariant().Trim(),
            DateOfBirth = dateOfBirth,
            CreatedAtUtc = DateTime.UtcNow
        };
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

    public void LinkToUser(Guid userId)
    {
        ThrowIfDeleted();
        UserId = userId;
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

    public void SoftDelete()
    {
        if (IsDeleted)
            throw DomainException.AlreadyDeleted(nameof(Member), Id);

        foreach (var membership in Memberships.Where(m => m.IsValid()))
        {
            membership.Cancel();
        }

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw DomainException.NotDeleted(nameof(Member), Id);

        IsDeleted = false;
        DeletedAtUtc = null;
    }

    private void ThrowIfDeleted()
    {
        if (IsDeleted)
            throw DomainException.EntityDeleted(nameof(Member), Id);
    }
}
