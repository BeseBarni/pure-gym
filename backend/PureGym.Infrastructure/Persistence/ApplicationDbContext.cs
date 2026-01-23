using Microsoft.EntityFrameworkCore;
using PureGym.Application.Interfaces;
using PureGym.Domain.Entities;
using PureGym.Infrastructure.Persistence.Extensions;

namespace PureGym.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<MembershipType> MembershipTypes => Set<MembershipType>();
    public DbSet<GymAccessLog> GymAccessLogs => Set<GymAccessLog>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.ApplySoftDeleteQueryFilters();
    }
}
