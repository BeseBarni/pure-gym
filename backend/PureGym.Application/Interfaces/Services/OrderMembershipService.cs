using MassTransit;
using Microsoft.EntityFrameworkCore;
using PureGym.Application.Interfaces;
using PureGym.Application.Interfaces.Services;
using PureGym.Application.Models;
using PureGym.Domain.Entities;
using PureGym.Domain.Enums;
using PureGym.SharedKernel.Events;
using PureGym.SharedKernel.Models;

namespace PureGym.Application.Services;

public sealed class OrderMembershipService : IOrderMembershipService
{
    private readonly IApplicationDbContext _db;
    private readonly IPublishEndpoint _publish;

    public OrderMembershipService(IApplicationDbContext db, IPublishEndpoint publish)
    {
        _db = db;
        _publish = publish;
    }

    public async Task<Result<Guid>> OrderAsync(Guid memberId, Guid membershipTypeId, CancellationToken ct)
    {
        var member = await _db.Members
            .Include(m => m.Memberships)
            .FirstOrDefaultAsync(m => m.Id == memberId, ct);

        if (member is null)
            return Result<Guid>.Failure(new Error("OM1", "Member not found."));

        var hasExisting = member.Memberships.Any(m =>
            !m.IsDeleted &&
            (m.Status == MembershipStatus.Active || m.Status == MembershipStatus.Pending) &&
            !m.IsExpired());

        if (hasExisting)
            return Result<Guid>.Failure(new Error("OM2", "Member already has an active or pending membership."));

        var type = await _db.MembershipTypes
            .FirstOrDefaultAsync(t => t.Id == membershipTypeId, ct);

        if (type is null)
            return Result<Guid>.Failure(new Error("OM3", "Membership type not found."));

        var membership = Membership.CreatePending(member, type);

        var order = MemberOrder.Create(member.Id, membership.Id, type.Id);
        _db.MemberOrders.Add(order);

        await _publish.Publish(
            new MembershipOrderedEvent(member.Id, membership.Id, DateTime.UtcNow),
            ct);

        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(membership.Id);
    }
}