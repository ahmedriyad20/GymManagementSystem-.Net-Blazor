namespace GymManagementSystem.DTOs.Reports.Results
{
    public class ExpiringSubscriptionResult
    {
        public Guid TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public Guid SubscriptionId { get; set; }
        public DateTime EndDate { get; set; }
        public int RemainingDays { get; set; }
    }
}
