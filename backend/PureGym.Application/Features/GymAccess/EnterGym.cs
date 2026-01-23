using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PureGym.Application.Interfaces;
using PureGym.Domain.Entities;
using PureGym.Domain.Enums;

namespace PureGym.Application.Features.GymAccess;

public static class EnterGym
{
    public sealed record Request(Guid MemberId);

    public sealed record Command(Guid MemberId) : IRequest<Response>;

    public sealed record Response(
        Guid AccessLogId,
        Guid MemberId,
        AccessResult Result,
        DateTime AccessedAtUtc,
        bool WasGranted);

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.MemberId)
                .NotEmpty()
                .WithMessage("Member ID is required.");
        }
    }

    public sealed class Handler : IRequestHandler<Command, Response>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var member = await _context.Members
                .Include(m => m.Memberships)
                    .ThenInclude(ms => ms.MembershipType)
                .FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken);

            if (member is null)
            {
                throw new KeyNotFoundException($"Member with ID '{request.MemberId}' was not found.");
            }

            var accessLog = GymAccessLog.Record(member);

            _context.GymAccessLogs.Add(accessLog);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response(
                accessLog.Id,
                member.Id,
                accessLog.Result,
                accessLog.AccessedAtUtc,
                accessLog.WasGranted());
        }
    }
}
