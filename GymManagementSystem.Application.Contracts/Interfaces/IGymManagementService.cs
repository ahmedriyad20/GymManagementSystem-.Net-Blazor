using GymManagementSystem.DTOs.Attendance.Commands;
using GymManagementSystem.DTOs.Expense.Commands;
using GymManagementSystem.DTOs.Expense.Results;
using GymManagementSystem.DTOs.Reports.Results;
using GymManagementSystem.DTOs.Subscription.Commands;
using GymManagementSystem.DTOs.Subscription.Results;

namespace GymManagementSystem.Interfaces
{
    public interface IGymManagementService
    {
        Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionCommand command, string currentUserId);
        Task<SubscriptionResult?> AddInstallmentAsync(Guid subscriptionId, AddInstallmentCommand command, string currentUserId);
        Task<bool> AddAttendanceSessionAsync(CreateAttendanceSessionCommand command, string currentUserId);
        Task<Guid> AddExpenseAsync(CreateExpenseCommand command);
        Task<List<ExpenseResult>> GetExpensesAsync(DateTime? fromDate, DateTime? toDate);
        Task<List<ExpiringSubscriptionResult>> GetExpiringSubscriptionsAsync(int days, string currentUserId);
        Task<List<UnpaidInstallmentResult>> GetUnpaidInstallmentsAsync(string currentUserId);
        Task<EarningsSummaryResult> GetEarningsSummaryAsync(int year, int month);
    }
}
