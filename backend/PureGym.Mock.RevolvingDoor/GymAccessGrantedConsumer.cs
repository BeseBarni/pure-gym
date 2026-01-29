using MassTransit;
using PureGym.SharedKernel.Events;

namespace PureGym.Mock.RevolvingDoor;

public class GymAccessGrantedConsumer : IConsumer<GymAccessGrantedEvent>
{
    private readonly DoorState _state;
    private readonly ILogger<GymAccessGrantedConsumer> _logger;

    public GymAccessGrantedConsumer(DoorState state, ILogger<GymAccessGrantedConsumer> logger)
    {
        _state = state;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<GymAccessGrantedEvent> context)
    {
        _logger.LogInformation("DOOR: User {UserId} is at the turnstile.", context.Message.MemberId);

        // Put them in the "active" slot
        _state.SetUser(context.Message);

        return Task.CompletedTask;
    }
}
