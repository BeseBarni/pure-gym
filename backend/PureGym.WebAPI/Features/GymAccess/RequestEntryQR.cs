using FastEndpoints;
using MediatR;
using PureGym.Application.Features.GymAccess;

namespace PureGym.WebAPI.Features.GymAccess;

public sealed class RequestEntryQRRequest
{
    public Guid MemberId { get; set; }
}
public sealed class RequestEntryQREndpoint : Endpoint<RequestEntryQRRequest, RequestEntryQR.Response>
{
    private readonly IMediator _mediator;

    public RequestEntryQREndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/gym/members/{MemberId:guid}/access-key");
        AllowAnonymous();
        Summary(s =>
        {
            s.Description = "Generates a QR entry key for a specific member.";
        });
    }

    public override async Task HandleAsync(RequestEntryQRRequest reg, CancellationToken ct)
    {
        var command = new RequestEntryQR.Command(reg.MemberId);

        var result = await _mediator.Send(command, ct);

        await Send.OkAsync(result);
    }
}