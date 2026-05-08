using GymManagementSystem.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.Entities
{
    public class Subscription : BaseEntity
    {
        public Guid TraineeId { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; } 
        public decimal TotalAmount { get; set; }

        public enSubscriptionPlan SubscriptionPlan { get; set; }
        public enSubscriptionPeriod SubscriptionPeriod { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Trainee Trainee { get; set; } = null!;
        public ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
    }
}
