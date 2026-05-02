using GymManagementSystem.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DTOs.Trainee.Results
{
    public class GetAllTraineesResult
    {
        public Guid TraineeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public string SubscriptionPlan { get; set; } = string.Empty;
        public string SubscriptionPeriod { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
