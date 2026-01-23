using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PureGym.Domain.Entities;

namespace PureGym.Infrastructure.Persistence.Configurations;

public class GymAccessLogConfiguration : IEntityTypeConfiguration<GymAccessLog>
{
    public void Configure(EntityTypeBuilder<GymAccessLog> builder)
    {
        builder.ToTable("gym_access_logs");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("id");

        builder.Property(g => g.MemberId)
            .HasColumnName("member_id")
            .IsRequired();

        builder.Property(g => g.MembershipId)
            .HasColumnName("membership_id");

        builder.Property(g => g.AccessedAtUtc)
            .HasColumnName("accessed_at_utc")
            .IsRequired();

        builder.Property(g => g.Result)
            .HasColumnName("result")
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne(g => g.Member)
            .WithMany(m => m.AccessLogs)
            .HasForeignKey(g => g.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Membership)
            .WithMany(m => m.AccessLogs)
            .HasForeignKey(g => g.MembershipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(g => g.MemberId);
        builder.HasIndex(g => g.MembershipId);
        builder.HasIndex(g => g.AccessedAtUtc);
        builder.HasIndex(g => g.Result);
        builder.HasIndex(g => new { g.MemberId, g.AccessedAtUtc });
        builder.HasIndex(g => new { g.MembershipId, g.AccessedAtUtc });
    }
}
