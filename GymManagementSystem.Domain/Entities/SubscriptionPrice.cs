using GymManagementSystem.Enums;

namespace GymManagementSystem.Entities
{
    public class SubscriptionPrice : BaseEntity
    {
        public enSubscriptionPlan SubscriptionPlan { get; set; }
        public enSubscriptionPeriod SubscriptionPeriod { get; set; }
        public decimal Price { get; set; }
    }
}
