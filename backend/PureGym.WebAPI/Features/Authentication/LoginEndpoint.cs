using FastEndpoints;
using MediatR;
using PureGym.Application.Features.Authentication;

namespace PureGym.WebAPI.Features.Authentication;

public sealed class LoginEndpoint : Endpoint<Login.Command, Login.Response>
{
    private readonly IMediator _mediator;

    public LoginEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Authenticate a user";
            s.Description = "Authenticates a user with email and password and returns a JWT token.";
        });
    }

    public override async Task HandleAsync(Login.Command req, CancellationToken ct)
    {
        var result = await _mediator.Send(req, ct);
        Response = result;
    }
}
