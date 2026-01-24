using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IApplicationDbContext _context;

        public Handler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _context = context;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            var member = Member.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.DateOfBirth);

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                member.UpdateContactInfo(request.PhoneNumber);
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAtUtc = DateTime.UtcNow,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            member.LinkToUser(user.Id);
            _context.Members.Add(member);
            await _context.SaveChangesAsync(cancellationToken);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenService.GenerateToken(user.Id, user.Email!, roles);

            return new Response(user.Id, user.Email!, token);
        }
    }
}
