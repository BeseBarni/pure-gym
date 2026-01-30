using FastEndpoints;
using MediatR;
using PureGym.Application.Features.Common;
using PureGym.Domain.Entities;

namespace PureGym.WebAPI.Features.GymAccess;

public sealed class ListGymAccessLogsEndpoint : EndpointWithoutRequest<ListQuery<GymAccessLog>.Response>
{
    private readonly IMediator _mediator;

    public ListGymAccessLogsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/gym-access-logs");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all gym access logs";
            s.Description = "Returns a list of all gym access logs.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new ListQuery<GymAccessLog>.Query();
        var result = await _mediator.Send(query, ct);
        Response = result.Value;
    }
}
