using FastEndpoints;
using MediatR;
using PureGym.Application.Features.Common;
using PureGym.Domain.Entities;

namespace PureGym.WebAPI.Features.MembershipTypes;

public sealed class ListMembershipTypesEndpoint : EndpointWithoutRequest<ListQuery<MembershipType>.Response>
{
    private readonly IMediator _mediator;

    public ListMembershipTypesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/membership-types");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all membership types";
            s.Description = "Returns a list of all active (non-deleted) membership types.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new ListQuery<MembershipType>.Query();
        var result = await _mediator.Send(query, ct);
        Response = result.Value;
    }
}
