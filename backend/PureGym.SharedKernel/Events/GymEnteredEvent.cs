using MediatR;

namespace PureGym.SharedKernel.Events;

public record GymEnteredEvent(
    Guid MemberId,
    DateTime OccurredOnUtc
) : INotification;
