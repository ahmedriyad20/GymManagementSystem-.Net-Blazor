using System;

namespace GymManagementSystem.Entities
{
    public class Installment : BaseEntity
    {
        public Guid SubscriptionId { get; set; }
        public decimal Amount { get; set; }

        public Subscription Subscription { get; set; } = null!;
    }
}
