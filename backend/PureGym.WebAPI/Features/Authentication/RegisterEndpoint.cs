using FastEndpoints;
using MediatR;
using PureGym.Application.Features.Authentication;

namespace PureGym.WebAPI.Features.Authentication;

public sealed class RegisterEndpoint : Endpoint<Register.Command, Register.Response>
{
    private readonly IMediator _mediator;

    public RegisterEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Register a new user";
            s.Description = "Creates a new user account and member profile, then returns a JWT token.";
        });
    }

    public override async Task HandleAsync(Register.Command req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        Response = result;
    }
}
