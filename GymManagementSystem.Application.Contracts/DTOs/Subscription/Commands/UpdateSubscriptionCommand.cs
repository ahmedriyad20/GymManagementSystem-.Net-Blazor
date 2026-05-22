using GymManagementSystem.Enums;

namespace GymManagementSystem.DTOs.Subscription.Commands
{
    public class UpdateSubscriptionCommand
    {
        public enSubscriptionPlan SubscriptionPlan { get; set; }
        public enSubscriptionPeriod SubscriptionPeriod { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime StartDate { get; set; }
    }
}
