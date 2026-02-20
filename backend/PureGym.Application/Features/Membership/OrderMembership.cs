using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MediatR;
using PureGym.Application.Interfaces;
using PureGym.Application.Models;
using PureGym.Domain.Entities;
using PureGym.Domain.Enums;
using PureGym.SharedKernel.Events;

namespace PureGym.Application.Features.Membership;

public static class OrderMembership
{
    public sealed record Request(Guid MemberId, Guid MembershipTypeId);

    public sealed record Command(Guid MemberId, Guid MembershipTypeId)
        : IRequest<Result<Response>>;

    public sealed record Response(Guid MembershipId);

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.MemberId).NotEmpty();
            RuleFor(x => x.MembershipTypeId).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result<Response>>
    {
        private readonly IApplicationDbContext _db;
        private readonly IPublishEndpoint _publish;

        public Handler(IApplicationDbContext db, IPublishEndpoint publish)
        {
            _db = db;
            _publish = publish;
        }

        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var member = await _db.Members
                .Include(m => m.Memberships)
                .FirstOrDefaultAsync(m => m.Id == request.MemberId, ct);

            if (member is null)
                return Result<Response>.Failure(new Error("OM1", "Member not found."));

            var hasExisting = member.Memberships.Any(m =>
                !m.IsDeleted &&
                (m.Status == MembershipStatus.Active || m.Status == MembershipStatus.Pending) &&
                !m.IsExpired()
            );

            if (hasExisting)
                return Result<Response>.Failure(new Error("OM2", "Member already has an active or pending membership."));

            var type = await _db.MembershipTypes
                .FirstOrDefaultAsync(t => t.Id == request.MembershipTypeId, ct);

            if (type is null)
                return Result<Response>.Failure(new Error("OM3", "Membership type not found."));

            var membership = PureGym.Domain.Entities.Membership.CreatePending(member, type);
            var order = MemberOrder.Create(
                memberId: member.Id,
                membershipId: membership.Id,
                membershipTypeId: type.Id
            );

            _db.MemberOrders.Add(order);

            await _db.SaveChangesAsync(ct);

            await _publish.Publish(
                new MembershipOrderedEvent(member.Id, membership.Id, DateTime.UtcNow),
                ct);

            return Result<Response>.Success(new Response(membership.Id));
        }
    }
}