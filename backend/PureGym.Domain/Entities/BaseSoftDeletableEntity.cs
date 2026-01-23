using PureGym.Domain.Exceptions;
using PureGym.Domain.Interfaces;

namespace PureGym.Domain.Entities;

public abstract class BaseSoftDeletableEntity : ISoftDeletable
{
    public abstract Guid Id { get; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    protected abstract string EntityName { get; }

    public virtual void SoftDelete()
    {
        if (IsDeleted)
            throw DomainException.AlreadyDeleted(EntityName, Id);

        OnBeforeSoftDelete();

        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
    }

    public virtual void Restore()
    {
        if (!IsDeleted)
            throw DomainException.NotDeleted(EntityName, Id);

        IsDeleted = false;
        DeletedAtUtc = null;

        OnAfterRestore();
    }

    protected void ThrowIfDeleted()
    {
        if (IsDeleted)
            throw DomainException.EntityDeleted(EntityName, Id);
    }

    protected virtual void OnBeforeSoftDelete() { }
    protected virtual void OnAfterRestore() { }
}
