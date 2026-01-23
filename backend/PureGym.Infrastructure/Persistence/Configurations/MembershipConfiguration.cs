using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PureGym.Domain.Entities;

namespace PureGym.Infrastructure.Persistence.Configurations;

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("memberships");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id");

        builder.Property(m => m.MemberId)
            .HasColumnName("member_id")
            .IsRequired();

        builder.Property(m => m.MembershipTypeId)
            .HasColumnName("membership_type_id")
            .IsRequired();

        builder.Property(m => m.StartDateUtc)
            .HasColumnName("start_date_utc")
            .IsRequired();

        builder.Property(m => m.EndDateUtc)
            .HasColumnName("end_date_utc")
            .IsRequired();

        builder.Property(m => m.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(m => m.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.DeletedAtUtc)
            .HasColumnName("deleted_at_utc");

        builder.HasOne(m => m.Member)
            .WithMany(m => m.Memberships)
            .HasForeignKey(m => m.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.MembershipType)
            .WithMany(mt => mt.Memberships)
            .HasForeignKey(m => m.MembershipTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.MemberId);
        builder.HasIndex(m => m.MembershipTypeId);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.EndDateUtc);
        builder.HasIndex(m => m.IsDeleted);

        builder.Ignore(m => m.DaysRemaining);
    }
}
