namespace GymManagementSystem.DTOs.SubscriptionPrice.Results
{
    public class GetSubscriptionPriceResult
    {
        public Guid SubscriptionPriceId { get; set; }
        public string SubscriptionPlan { get; set; } = string.Empty;
        public string SubscriptionPeriod { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
