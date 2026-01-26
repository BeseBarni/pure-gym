using FluentValidation;
using MediatR;
using PureGym.Application.Interfaces;
using PureGym.Domain.Entities;

namespace PureGym.Application.Features.Authentication;

public static class Register
{
    public sealed record Command(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        string? PhoneNumber = null) : IRequest<Response>;

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

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("First name is required and must not exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Last name is required and must not exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .LessThan(DateTime.UtcNow.AddYears(-16))
                .WithMessage("User must be at least 16 years old.");
        }
    }

    public sealed class Handler : IRequestHandler<Command, Response>
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IApplicationDbContext _context;

        public Handler(IAuthService authService, IJwtTokenService jwtTokenService, IApplicationDbContext context)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _context = context;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            if (await _authService.EmailExistsAsync(request.Email)) throw new InvalidOperationException("User with this email already exists.");

            var (success, userId, errors) = await _authService.CreateUserAsync(request.Email, request.Password);

            if (!success) throw new Exception(string.Join(", ", errors));

            var member = Member.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.DateOfBirth);

            member.LinkToUser(userId);
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                member.UpdateContactInfo(request.PhoneNumber);
            }


            _context.Members.Add(member);
            await _context.SaveChangesAsync(cancellationToken);

            var token = _jwtTokenService.GenerateToken(userId, request.Email, []);

            return new Response(userId, request.Email, token);
        }
    }
}
