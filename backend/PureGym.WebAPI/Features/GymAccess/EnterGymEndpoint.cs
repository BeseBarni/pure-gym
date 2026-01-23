using FastEndpoints;
using MediatR;
using PureGym.Application.Features.GymAccess;

namespace PureGym.WebAPI.Features.GymAccess;

public sealed class EnterGymEndpoint : Endpoint<EnterGym.Request, EnterGym.Response>
{
    public override void Configure()
    {
        Post("/api/gym/enter");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Record a gym entry attempt";
            s.Description = "Records an attempt by a member to enter the gym and returns whether access was granted.";
        });
    }

    public override async Task HandleAsync(EnterGym.Request req, CancellationToken ct)
    {
        var mediator = Resolve<IMediator>();
        var command = new EnterGym.Command(req.MemberId);
        var result = await mediator.Send(command, ct);
        Response = result;
    }
}
