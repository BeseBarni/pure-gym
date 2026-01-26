using FluentValidation;
using MediatR;
using PureGym.Application.Interfaces.Services;

namespace PureGym.Application.Features.Authentication;

public static class Login
{
    public sealed record Command(
    string Email,
    string Password) : IRequest<Response>;

    public sealed record Response(
        Guid UserId,
        string Email,
        string Token);

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long.");
        }
    }

    public sealed class Handler : IRequestHandler<Command, Response>
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public Handler(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var (success, userId, roles) = await _authService.CheckPasswordAsync(request.Email, request.Password);

            if (!success)
                throw new Exception("Invalid email or password.");

            var token = _jwtTokenService.GenerateToken(userId, request.Email, roles);

            return new Response(userId, request.Email, token);
        }
    }
}
