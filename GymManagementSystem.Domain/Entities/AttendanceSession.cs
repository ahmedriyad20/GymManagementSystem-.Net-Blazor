using System;

namespace GymManagementSystem.Entities
{
    public class AttendanceSession : BaseEntity
    {
        public Guid TraineeId { get; set; }
        public Guid SubscriptionId { get; set; }
        public DateTime SessionDateTime { get; set; }

        public Trainee Trainee { get; set; } = null!;
        public Subscription Subscription { get; set; } = null!;
    }
}
