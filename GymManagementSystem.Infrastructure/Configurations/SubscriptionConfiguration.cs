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
            builder.Property(s => s.TotalAmount)
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

            builder.HasOne(s => s.Trainee)
                .WithMany(t => t.Subscriptions)
                .HasForeignKey(s => s.TraineeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.AttendanceSessions)
                .WithOne(a => a.Subscription)
                .HasForeignKey(a => a.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
