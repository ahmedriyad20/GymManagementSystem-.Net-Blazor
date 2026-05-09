namespace GymManagementSystem.DTOs.Expense.Commands
{
    public class CreateExpenseCommand
    {
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? PaidBy { get; set; }
        public string? Notes { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}
