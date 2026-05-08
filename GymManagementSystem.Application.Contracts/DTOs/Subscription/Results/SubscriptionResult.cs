namespace GymManagementSystem.DTOs.Subscription.Results
{
    public class SubscriptionResult
    {
        public Guid SubscriptionId { get; set; }
        public Guid TraineeId { get; set; }
        public string SubscriptionPlan { get; set; } = string.Empty;
        public string SubscriptionPeriod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
