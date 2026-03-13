using FastEndpoints;
using MediatR;
using PureGym.Application.Features.Membership;
using PureGym.SharedKernel.Models;

namespace PureGym.WebAPI.Features.Membership;

public sealed class AddMembershipEndpoint : Endpoint<AddMembership.Request, Result<AddMembership.Response>>
{
    public override void Configure()
    {
        Post("/membership");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Add a membership to a member";
            s.Description = "Creates a new membership for a member based on the specified membership type.";
        });
    }

    public override async Task HandleAsync(AddMembership.Request req, CancellationToken ct)
    {
        var mediator = Resolve<IMediator>();
        var command = new AddMembership.Command(req.MemberId, req.MembershipTypeId);
        var result = await mediator.Send(command, ct);
        Response = result;
    }
}
