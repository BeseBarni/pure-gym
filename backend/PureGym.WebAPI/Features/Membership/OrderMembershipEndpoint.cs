using FastEndpoints;
using MediatR;
using PureGym.Application.Features.Membership;
using PureGym.Application.Models;
using PureGym.SharedKernel.Models;

namespace PureGym.WebAPI.Features.Membership;

public sealed class OrderMembershipEndpoint
    : Endpoint<OrderMembership.Request, Result<OrderMembership.Response>>
{
    public override void Configure()
    {
        Post("/membership/order");
        Summary(s =>
        {
            s.Summary = "Order membership";
            s.Description = "Creates a pending membership and publishes MembershipOrderedEvent.";
        });
    }

    public override async Task HandleAsync(OrderMembership.Request req, CancellationToken ct)
    {
        var mediator = Resolve<IMediator>();
        var command = new OrderMembership.Command(req.MemberId, req.MembershipTypeId);

        var result = await mediator.Send(command, ct);
        Response = result;
    }
}