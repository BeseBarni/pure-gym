using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PureGym.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PureGym.Infrastructure.Persistence.Configurations
{
    public sealed class MemberOrderConfiguration : IEntityTypeConfiguration<MemberOrder>
    {
        public void Configure(EntityTypeBuilder<MemberOrder> builder) 
        {
            builder.ToTable("member_orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MemberId).IsRequired();
            builder.Property(x => x.MembershipId).IsRequired();
            builder.Property(x => x.OrderedAtUtc).IsRequired();

            builder.HasOne(x => x.Member)
                .WithMany(m => m.Orders)
                .HasForeignKey(x => x.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
