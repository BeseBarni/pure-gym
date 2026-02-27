using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PureGym.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PureGym.Infrastructure.Persistence.Configurations;

public sealed class MemberOrderConfiguration : IEntityTypeConfiguration<MemberOrder>
{
    public void Configure(EntityTypeBuilder<MemberOrder> builder) 
    {
        builder.ToTable("member_orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MemberId).IsRequired();
        builder.Property(x => x.MembershipId).IsRequired();
        builder.Property(x => x.MembershipTypeId).IsRequired();
        builder.Property(x => x.OrderedAtUtc).IsRequired();

        builder.HasOne(x => x.Member)
            .WithMany(m => m.Orders)
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Membership)
            .WithOne(m => m.Orders)
            .HasForeignKey<MemberOrder>(x => x.MembershipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.MembershipType)
            .WithMany(mt => mt.Orders)
            .HasForeignKey(x => x.MembershipTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.MemberId).IsUnique();
        builder.HasIndex(x => x.MembershipId);
        builder.HasIndex(x => x.OrderedAtUtc);

    }
}
