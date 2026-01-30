using FastEndpoints;
using MediatR;
using PureGym.Application.Features.Common;

namespace PureGym.WebAPI.Features.Membership;

public sealed class ListMembershipsEndpoint : EndpointWithoutRequest<ListQuery<Domain.Entities.Membership>.Response>
{
    private readonly IMediator _mediator;

    public ListMembershipsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/memberships");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all memberships";
            s.Description = "Returns a list of all active (non-deleted) memberships.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new ListQuery<Domain.Entities.Membership>.Query();
        var result = await _mediator.Send(query, ct);
        Response = result.Value;
    }
}
