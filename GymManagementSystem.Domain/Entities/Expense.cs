using System;

namespace GymManagementSystem.Entities
{
    public class Expense : BaseEntity
    {
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string PaidBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}
