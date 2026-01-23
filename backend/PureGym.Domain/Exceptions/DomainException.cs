namespace PureGym.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainErrorCode ErrorCode { get; }
    public object? Context { get; }

    public DomainException(DomainErrorCode errorCode, string message, object? context = null)
        : base(message)
    {
        ErrorCode = errorCode;
        Context = context;
    }

    // Entity deleted errors
    public static DomainException EntityDeleted(string entityName, Guid id) =>
        new(DomainErrorCode.EntityDeleted,
            $"Cannot perform operation on deleted {entityName}",
            new { EntityName = entityName, EntityId = id });

    public static DomainException AlreadyDeleted(string entityName, Guid id) =>
        new(DomainErrorCode.EntityAlreadyDeleted,
            $"{entityName} is already deleted",
            new { EntityName = entityName, EntityId = id });

    public static DomainException NotDeleted(string entityName, Guid id) =>
        new(DomainErrorCode.EntityNotDeleted,
            $"{entityName} is not deleted",
            new { EntityName = entityName, EntityId = id });

    // State errors
    public static DomainException InvalidState(string entityName, string currentState, string operation) =>
        new(DomainErrorCode.InvalidEntityState,
            $"Cannot {operation} on {entityName}. Current state: {currentState}",
            new { EntityName = entityName, CurrentState = currentState, Operation = operation });

    // Validation errors
    public static DomainException Validation(string propertyName, string message) =>
        new(DomainErrorCode.ValidationFailed,
            message,
            new { PropertyName = propertyName });

    public static DomainException Required(string propertyName) =>
        new(DomainErrorCode.RequiredFieldMissing,
            $"{propertyName} is required",
            new { PropertyName = propertyName });

    public static DomainException InvalidEmail(string email) =>
        new(DomainErrorCode.InvalidEmail,
            "Invalid email format",
            new { Email = email });

    public static DomainException InvalidPrice(decimal price) =>
        new(DomainErrorCode.InvalidPrice,
            "Price cannot be negative",
            new { Price = price });

    public static DomainException InvalidDuration(int days) =>
        new(DomainErrorCode.InvalidDuration,
            "Duration must be positive",
            new { DurationInDays = days });

    // Business rule errors
    public static DomainException MembershipTypeInactive(Guid typeId) =>
        new(DomainErrorCode.MembershipTypeInactive,
            "Cannot create membership with inactive type",
            new { MembershipTypeId = typeId });

    public static DomainException MembershipExpired(Guid membershipId) =>
        new(DomainErrorCode.MembershipExpired,
            "Cannot reactivate expired membership",
            new { MembershipId = membershipId });

    public static DomainException MembershipAlreadyCancelled(Guid membershipId) =>
        new(DomainErrorCode.MembershipAlreadyCancelled,
            "Membership is already cancelled",
            new { MembershipId = membershipId });
}
