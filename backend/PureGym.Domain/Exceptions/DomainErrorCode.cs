namespace PureGym.Domain.Exceptions;

public enum DomainErrorCode
{
    // Validation errors (1xx)
    ValidationFailed = 100,
    InvalidEmail = 101,
    InvalidPrice = 102,
    InvalidDuration = 103,
    RequiredFieldMissing = 104,

    // Entity state errors (2xx)
    EntityDeleted = 200,
    EntityAlreadyDeleted = 201,
    EntityNotDeleted = 202,
    InvalidEntityState = 203,

    // Business rule errors (3xx)
    MembershipTypeInactive = 300,
    MembershipExpired = 301,
    MembershipAlreadyCancelled = 302,
    MembershipAlreadyActive = 303,
    CannotEnterGym = 304
}
