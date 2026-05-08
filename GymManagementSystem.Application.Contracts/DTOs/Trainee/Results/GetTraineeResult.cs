namespace GymManagementSystem.DTOs.Trainee.Results
{
    public class GetTraineeResult
    {
        public Guid TraineeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationTime { get; set; }
        public List<TraineeSubscriptionView> Subscriptions { get; set; } = new();
        public List<DateTime> AttendanceSessions { get; set; } = new();
    }

    public class TraineeSubscriptionView
    {
        public Guid SubscriptionId { get; set; }
        public string SubscriptionPlan { get; set; } = string.Empty;
        public string SubscriptionPeriod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
