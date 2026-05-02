using GymManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Subscription> builder) 
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.PaidAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(s => s.RemainingAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(s => s.SubscriptionPlan)
                .IsRequired();
            builder.Property(s => s.SubscriptionPeriod)
                .IsRequired();
            builder.Property(s => s.StartDate)
                .IsRequired();
            builder.Property(s => s.EndDate)
                .IsRequired();
        }
    }
}
