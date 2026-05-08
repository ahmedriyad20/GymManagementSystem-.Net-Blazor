using GymManagementSystem.Enums;

namespace GymManagementSystem.DTOs.SubscriptionPrice.Commands
{
    public class CreateSubscriptionPriceCommand
    {
        public enSubscriptionPlan SubscriptionPlan { get; set; }
        public enSubscriptionPeriod SubscriptionPeriod { get; set; }
        public decimal Price { get; set; }
    }
}
