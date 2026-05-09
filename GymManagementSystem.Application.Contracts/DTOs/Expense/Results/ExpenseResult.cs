namespace GymManagementSystem.DTOs.Expense.Results
{
    public class ExpenseResult
    {
        public Guid ExpenseId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string PaidBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}
