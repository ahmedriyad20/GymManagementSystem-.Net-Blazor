using System;

namespace GymManagementSystem.DTOs.Reports.Results
{
    public class GymPaymentTransactionResult
    {
        public DateTime Date { get; set; }
        public string TraineeName { get; set; } = string.Empty;
        public string Plan { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty; // "Subscription" / "Installment"
    }
}
