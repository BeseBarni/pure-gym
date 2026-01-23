using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PureGym.Domain.Entities;

namespace PureGym.Infrastructure.Persistence.Configurations;

public class MembershipTypeConfiguration : IEntityTypeConfiguration<MembershipType>
{
    public void Configure(EntityTypeBuilder<MembershipType> builder)
    {
        builder.ToTable("membership_types");

        builder.HasKey(mt => mt.Id);

        builder.Property(mt => mt.Id)
            .HasColumnName("id");

        builder.Property(mt => mt.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mt => mt.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(mt => mt.PricePerMonth)
            .HasColumnName("price_per_month")
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(mt => mt.DurationInDays)
            .HasColumnName("duration_in_days")
            .IsRequired();

        builder.Property(mt => mt.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(mt => mt.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(mt => mt.DeletedAtUtc)
            .HasColumnName("deleted_at_utc");

        // PostgreSQL partial unique index
        builder.HasIndex(mt => mt.Name)
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(mt => mt.IsActive);
        builder.HasIndex(mt => mt.IsDeleted);
    }
}
