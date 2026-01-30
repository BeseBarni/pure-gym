using Microsoft.EntityFrameworkCore;
using PureGym.Domain.Entities;

namespace PureGym.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Member> Members { get; }
    DbSet<Membership> Memberships { get; }
    DbSet<MembershipType> MembershipTypes { get; }
    DbSet<GymAccessLog> GymAccessLogs { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
