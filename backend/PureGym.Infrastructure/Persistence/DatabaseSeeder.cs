using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PureGym.Domain.Entities;
using PureGym.Domain.Enums;

namespace PureGym.Infrastructure.Persistence;

public class DatabaseSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await context.Members.AnyAsync(cancellationToken))
            return;

        await SeedTestDataAsync(context, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task SeedTestDataAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        // Create a membership type
        var monthlyMembership = MembershipType.Create(
            name: "Monthly Premium",
            pricePerMonth: 29.99m,
            durationInDays: 30,
            description: "Full access to all gym facilities");

        context.MembershipTypes.Add(monthlyMembership);

        // Member 1: Valid membership
        var memberWithValidMembership = Member.Create(
            firstName: "John",
            lastName: "Active",
            email: "john.active@example.com",
            dateOfBirth: new DateTime(1990, 5, 15));

        var validMembership = CreateMembershipWithDates(
            member: memberWithValidMembership,
            membershipType: monthlyMembership,
            startDate: DateTime.UtcNow.AddDays(-10),
            endDate: DateTime.UtcNow.AddDays(20),
            status: MembershipStatus.Active);

        context.Members.Add(memberWithValidMembership);
        context.Memberships.Add(validMembership);

        // Member 2: Expired membership
        var memberWithExpiredMembership = Member.Create(
            firstName: "Jane",
            lastName: "Expired",
            email: "jane.expired@example.com",
            dateOfBirth: new DateTime(1985, 8, 22));

        var expiredMembership = CreateMembershipWithDates(
            member: memberWithExpiredMembership,
            membershipType: monthlyMembership,
            startDate: DateTime.UtcNow.AddDays(-60),
            endDate: DateTime.UtcNow.AddDays(-30),
            status: MembershipStatus.Active);

        context.Members.Add(memberWithExpiredMembership);
        context.Memberships.Add(expiredMembership);

        // Member 3: No membership
        var memberWithNoMembership = Member.Create(
            firstName: "Bob",
            lastName: "NoMembership",
            email: "bob.nomembership@example.com",
            dateOfBirth: new DateTime(1995, 3, 10));

        context.Members.Add(memberWithNoMembership);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static Membership CreateMembershipWithDates(
        Member member,
        MembershipType membershipType,
        DateTime startDate,
        DateTime endDate,
        MembershipStatus status)
    {
        // Use reflection to create membership with specific dates for testing
        var membership = (Membership)Activator.CreateInstance(typeof(Membership), nonPublic: true)!;

        typeof(Membership).GetProperty(nameof(Membership.MemberId))!
            .SetValue(membership, member.Id);
        typeof(Membership).GetProperty(nameof(Membership.MembershipTypeId))!
            .SetValue(membership, membershipType.Id);
        typeof(Membership).GetProperty(nameof(Membership.StartDateUtc))!
            .SetValue(membership, startDate);
        typeof(Membership).GetProperty(nameof(Membership.EndDateUtc))!
            .SetValue(membership, endDate);
        typeof(Membership).GetProperty(nameof(Membership.Status))!
            .SetValue(membership, status);
        typeof(Membership).GetProperty(nameof(Membership.CreatedAtUtc))!
            .SetValue(membership, DateTime.UtcNow);

        member.AddMembership(membership);

        return membership;
    }
}
