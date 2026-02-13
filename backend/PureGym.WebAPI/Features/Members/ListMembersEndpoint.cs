using FastEndpoints;
using MediatR;
using PureGym.Application.Features.Common;
using PureGym.Domain.Entities;

namespace PureGym.WebAPI.Features.Members;

public sealed class ListMembersEndpoint : EndpointWithoutRequest<ListQuery<Member>.Response>
{
    private readonly IMediator _mediator;

    public ListMembersEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/members");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all members";
            s.Description = "Returns a list of all active (non-deleted) members.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new ListQuery<Member>.Query();
        var result = await _mediator.Send(query, ct);
        Response = result.Value;
    }
}
