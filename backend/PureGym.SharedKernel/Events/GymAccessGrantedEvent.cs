namespace PureGym.SharedKernel.Events;

public record GymAccessGrantedEvent(
    Guid MemberId,
    DateTime OccurredOnUtc);
