using GymManagementSystem.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DTOs.Trainee.Commands
{
    public class CreateTraineeCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public enGender Gender { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public enSubscriptionPlan SubscriptionPlan { get; set; }
        public enSubscriptionPeriod SubscriptionPeriod { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
