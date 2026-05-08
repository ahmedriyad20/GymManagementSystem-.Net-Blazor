using GymManagementSystem.DTOs.Attendance.Commands;
using GymManagementSystem.DTOs.Expense.Commands;
using GymManagementSystem.DTOs.Expense.Results;
using GymManagementSystem.DTOs.Reports.Results;
using GymManagementSystem.DTOs.Subscription.Commands;
using GymManagementSystem.DTOs.Subscription.Results;
using GymManagementSystem.Entities;
using GymManagementSystem.Entities.Users;
using GymManagementSystem.Enums;
using GymManagementSystem.Interfaces;
using HireAI.Infrastructure.GenaricBasies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Services
{
    public class GymManagementService(
        IRepository<Trainee> traineeRepository,
        IRepository<Subscription> subscriptionRepository,
        IRepository<SubscriptionPrice> subscriptionPriceRepository,
        IRepository<AttendanceSession> attendanceSessionRepository,
        IRepository<Expense> expenseRepository,
        UserManager<User> userManager) : IGymManagementService
    {
        public async Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionCommand command, string currentUserId)
        {
            var trainee = await traineeRepository.GetAll().FirstOrDefaultAsync(t => t.Id == command.TraineeId)
                ?? throw new InvalidOperationException("Trainee not found.");

            await EnsureTraineeAccessAsync(trainee, currentUserId);

            var price = await subscriptionPriceRepository.GetAll()
                .FirstOrDefaultAsync(p => p.SubscriptionPlan == command.SubscriptionPlan && p.SubscriptionPeriod == command.SubscriptionPeriod)
                ?? throw new InvalidOperationException("Subscription price not configured for selected plan and period.");

            var totalAmount = price.Price;
            if (command.PaidAmount < 0 || command.PaidAmount > totalAmount)
            {
                throw new InvalidOperationException("Invalid paid amount.");
            }

            var subscription = new Subscription
            {
                TraineeId = command.TraineeId,
                SubscriptionPlan = command.SubscriptionPlan,
                SubscriptionPeriod = command.SubscriptionPeriod,
                TotalAmount = totalAmount,
                PaidAmount = command.PaidAmount,
                RemainingAmount = totalAmount - command.PaidAmount,
                StartDate = command.StartDate,
                EndDate = CalculateEndDate(command.StartDate, command.SubscriptionPeriod)
            };

            await subscriptionRepository.InsertAsync(subscription);
            return MapSubscription(subscription);
        }

        public async Task<SubscriptionResult?> AddInstallmentAsync(Guid subscriptionId, AddInstallmentCommand command, string currentUserId)
        {
            var subscription = await subscriptionRepository.GetAll()
                .Include(s => s.Trainee)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);

            if (subscription is null)
            {
                return null;
            }

            await EnsureTraineeAccessAsync(subscription.Trainee, currentUserId);

            if (command.AmountPaid <= 0 || command.AmountPaid > subscription.RemainingAmount)
            {
                throw new InvalidOperationException("Installment amount is invalid.");
            }

            subscription.PaidAmount += command.AmountPaid;
            subscription.RemainingAmount -= command.AmountPaid;

            await subscriptionRepository.UpdateAsync(subscription);
            return MapSubscription(subscription);
        }

        public async Task<bool> AddAttendanceSessionAsync(CreateAttendanceSessionCommand command, string currentUserId)
        {
            var subscription = await subscriptionRepository.GetAll()
                .Include(s => s.Trainee)
                .FirstOrDefaultAsync(s => s.Id == command.SubscriptionId && s.TraineeId == command.TraineeId);

            if (subscription is null)
            {
                return false;
            }

            await EnsureTraineeAccessAsync(subscription.Trainee, currentUserId);

            var session = new AttendanceSession
            {
                TraineeId = command.TraineeId,
                SubscriptionId = command.SubscriptionId,
                SessionDateTime = command.SessionDateTime
            };

            await attendanceSessionRepository.InsertAsync(session);
            return true;
        }

        public async Task<Guid> AddExpenseAsync(CreateExpenseCommand command)
        {
            var expense = new Expense
            {
                Description = command.Description,
                Amount = command.Amount,
                ExpenseDate = command.ExpenseDate
            };

            await expenseRepository.InsertAsync(expense);
            return expense.Id;
        }

        public async Task<List<ExpenseResult>> GetExpensesAsync(DateTime? fromDate, DateTime? toDate)
        {
            var query = expenseRepository.GetAll().AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(e => e.ExpenseDate.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(e => e.ExpenseDate.Date <= toDate.Value.Date);
            }

            var expenses = await query.OrderByDescending(e => e.ExpenseDate).ToListAsync();
            return expenses.Select(e => new ExpenseResult
            {
                ExpenseId = e.Id,
                Description = e.Description,
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate
            }).ToList();
        }

        public async Task<List<ExpiringSubscriptionResult>> GetExpiringSubscriptionsAsync(int days, string currentUserId)
        {
            var today = DateTime.UtcNow.Date;
            var maxDate = today.AddDays(days);
            var isFemaleOnly = await IsFemaleOnlyUserAsync(currentUserId);

            var query = subscriptionRepository.GetAll()
                .Include(s => s.Trainee)
                .Where(s => s.EndDate.Date >= today && s.EndDate.Date <= maxDate);

            if (isFemaleOnly)
            {
                query = query.Where(s => s.Trainee.Gender == enGender.Female);
            }

            var subscriptions = await query.OrderBy(s => s.EndDate).ToListAsync();
            return subscriptions.Select(s => new ExpiringSubscriptionResult
            {
                TraineeId = s.TraineeId,
                TraineeName = s.Trainee.Name,
                SubscriptionId = s.Id,
                EndDate = s.EndDate,
                RemainingDays = (s.EndDate.Date - today).Days
            }).ToList();
        }

        public async Task<List<UnpaidInstallmentResult>> GetUnpaidInstallmentsAsync(string currentUserId)
        {
            var isFemaleOnly = await IsFemaleOnlyUserAsync(currentUserId);

            var query = subscriptionRepository.GetAll()
                .Include(s => s.Trainee)
                .Where(s => s.RemainingAmount > 0);

            if (isFemaleOnly)
            {
                query = query.Where(s => s.Trainee.Gender == enGender.Female);
            }

            var subscriptions = await query.OrderByDescending(s => s.RemainingAmount).ToListAsync();
            return subscriptions.Select(s => new UnpaidInstallmentResult
            {
                TraineeId = s.TraineeId,
                TraineeName = s.Trainee.Name,
                SubscriptionId = s.Id,
                RemainingAmount = s.RemainingAmount
            }).ToList();
        }

        public async Task<EarningsSummaryResult> GetEarningsSummaryAsync(int year, int month)
        {
            var subscriptions = await subscriptionRepository.GetAll().ToListAsync();
            var total = subscriptions.Sum(s => s.PaidAmount);
            var monthly = subscriptions
                .Where(s => s.StartDate.Year == year && s.StartDate.Month == month)
                .Sum(s => s.PaidAmount);

            return new EarningsSummaryResult
            {
                TotalEarnings = total,
                MonthlyEarnings = monthly
            };
        }

        private static DateTime CalculateEndDate(DateTime startDate, enSubscriptionPeriod period)
        {
            return period switch
            {
                enSubscriptionPeriod.Daily => startDate.Date,
                enSubscriptionPeriod.Monthly => startDate.AddMonths(1).Date,
                enSubscriptionPeriod.SixMonths => startDate.AddMonths(6).Date,
                enSubscriptionPeriod.Yearly => startDate.AddYears(1).Date,
                _ => throw new InvalidOperationException("Unsupported subscription period.")
            };
        }

        private static SubscriptionResult MapSubscription(Subscription subscription)
        {
            return new SubscriptionResult
            {
                SubscriptionId = subscription.Id,
                TraineeId = subscription.TraineeId,
                SubscriptionPlan = subscription.SubscriptionPlan.ToString(),
                SubscriptionPeriod = subscription.SubscriptionPeriod.ToString(),
                TotalAmount = subscription.TotalAmount,
                PaidAmount = subscription.PaidAmount,
                RemainingAmount = subscription.RemainingAmount,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate
            };
        }

        private async Task EnsureTraineeAccessAsync(Trainee trainee, string currentUserId)
        {
            var isFemaleOnly = await IsFemaleOnlyUserAsync(currentUserId);
            if (isFemaleOnly && trainee.Gender != enGender.Female)
            {
                throw new UnauthorizedAccessException("You do not have access to this trainee.");
            }
        }

        private async Task<bool> IsFemaleOnlyUserAsync(string currentUserId)
        {
            var user = await userManager.FindByIdAsync(currentUserId);
            return user is not null && user.Gender == enGender.Female;
        }
    }
}
