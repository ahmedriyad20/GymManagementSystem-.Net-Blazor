namespace GymManagementSystem.DTOs.Expense.Results
{
    public class ExpenseResult
    {
        public Guid ExpenseId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}
