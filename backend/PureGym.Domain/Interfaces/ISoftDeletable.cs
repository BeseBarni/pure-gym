namespace PureGym.Domain.Interfaces;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAtUtc { get; }

    void SoftDelete();
    void Restore();
}
