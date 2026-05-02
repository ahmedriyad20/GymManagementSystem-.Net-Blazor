using GymManagementSystem.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.Entities
{
    public class Subscription : BaseEntity
    {
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; } 

        public enSubscriptionPlan SubscriptionPlan { get; set; }
        public enSubscriptionPeriod SubscriptionPeriod { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
