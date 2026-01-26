using FastEndpoints;
using MediatR;
using PureGym.Application.Features.GymAccess;

namespace PureGym.WebAPI.Features.GymAccess;

public sealed class RequestEntryQREndpoint : Endpoint<RequestEntryQR.Request, RequestEntryQR.Response>
{
    private readonly IMediator _mediator;
    public RequestEntryQREndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    public override void Configure()
    {
        Post("/gym/members/{MemberId}/access-keys");
    }

    public override async Task HandleAsync(RequestEntryQR.Request req, CancellationToken ct)
    {
        var command = new RequestEntryQR.Command(req.MemberId);
        var result = await _mediator.Send(command, ct);
        Response = result;
    }
}
