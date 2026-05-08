namespace GymManagementSystem.DTOs.Reports.Results
{
    public class UnpaidInstallmentResult
    {
        public Guid TraineeId { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public Guid SubscriptionId { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
