using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PureGym.Domain.Entities;

namespace PureGym.Infrastructure.Persistence.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id");

        builder.Property(m => m.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.LastName)
            .HasColumnName("last_name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(m => m.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20);

        builder.Property(m => m.DateOfBirth)
            .HasColumnName("date_of_birth")
            .IsRequired();

        builder.Property(m => m.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(m => m.UserId)
            .HasColumnName("user_id");

        builder.Property(m => m.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(m => m.DeletedAtUtc)
            .HasColumnName("deleted_at_utc");

        // PostgreSQL partial unique index
        builder.HasIndex(m => m.Email)
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(m => m.UserId)
            .HasFilter("user_id IS NOT NULL");

        builder.HasIndex(m => m.IsDeleted);

        builder.Ignore(m => m.FullName);
    }
}
