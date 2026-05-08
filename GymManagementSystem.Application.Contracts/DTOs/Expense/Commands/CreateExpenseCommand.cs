namespace GymManagementSystem.DTOs.Expense.Commands
{
    public class CreateExpenseCommand
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}
