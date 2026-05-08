using GymManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Configurations
{
    public class AttendanceSessionConfiguration : IEntityTypeConfiguration<AttendanceSession>
    {
        public void Configure(EntityTypeBuilder<AttendanceSession> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.SessionDateTime).IsRequired();

            builder.HasIndex(a => new { a.TraineeId, a.SubscriptionId, a.SessionDateTime });
        }
    }
}
