using GymManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.Configurations
{
    public class TraineeConfiguration : IEntityTypeConfiguration<Trainee>
    {
        public void Configure(EntityTypeBuilder<Trainee> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Phone).HasMaxLength(20);
            builder.Property(t => t.PhotoPath).HasMaxLength(500);
            builder.Property(t => t.DateOfBirth).IsRequired();
            builder.Property(t => t.IsActive).IsRequired();

            builder.HasMany(t => t.Subscriptions)
                .WithOne(s => s.Trainee)
                .HasForeignKey(s => s.TraineeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.AttendanceSessions)
                .WithOne(a => a.Trainee)
                .HasForeignKey(a => a.TraineeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
