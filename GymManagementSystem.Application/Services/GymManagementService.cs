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
        IRepository<Installment> installmentRepository,
        UserManager<User> userManager) : IGymManagementService
    {
        public async Task<SubscriptionResult> CreateSubscriptionAsync(CreateSubscriptionCommand command, string currentUserId)
        {
            var trainee = await traineeRepository.GetAll().FirstOrDefaultAsync(t => t.Id == command.TraineeId)
                ?? throw new InvalidOperationException("Trainee not found.");

            await EnsureTraineeAccessAsync(trainee, currentUserId);

            if (command.SubscriptionPrice <= 0)
            {
                throw new InvalidOperationException("Subscription price must be greater than zero.");
            }

            if (command.PaidAmount < 0 || command.PaidAmount > command.SubscriptionPrice)
            {
                throw new InvalidOperationException("Invalid paid amount.");
            }

            var subscription = new Subscription
            {
                TraineeId = command.TraineeId,
                SubscriptionPlan = command.SubscriptionPlan,
                SubscriptionPeriod = command.SubscriptionPeriod,
                SubscriptionPrice = command.SubscriptionPrice,
                TotalAmount = command.SubscriptionPrice,
                PaidAmount = command.PaidAmount,
                RemainingAmount = command.SubscriptionPrice - command.PaidAmount,
                StartDate = command.StartDate,
                EndDate = CalculateEndDate(command.StartDate, command.SubscriptionPeriod)
            };

            await subscriptionRepository.InsertAsync(subscription);

            if (command.PaidAmount > 0)
            {
                var installment = new Installment
                {
                    SubscriptionId = subscription.Id,
                    Amount = command.PaidAmount
                };
                installment.CreationTime = command.StartDate;
                await installmentRepository.InsertAsync(installment);
            }

            return MapSubscription(subscription);
        }

        public async Task<SubscriptionResult?> UpdateSubscriptionAsync(Guid subscriptionId, UpdateSubscriptionCommand command, string currentUserId)
        {
            var subscription = await subscriptionRepository.GetAll()
                .Include(s => s.Trainee)
                .FirstOrDefaultAsync(s => s.Id == subscriptionId);

            if (subscription is null)
            {
                return null;
            }

            await EnsureTraineeAccessAsync(subscription.Trainee, currentUserId);

            if (command.SubscriptionPrice <= 0)
            {
                throw new InvalidOperationException("Subscription price must be greater than zero.");
            }

            if (command.PaidAmount < 0 || command.PaidAmount > command.SubscriptionPrice)
            {
                throw new InvalidOperationException("Invalid paid amount.");
            }

            subscription.SubscriptionPlan = command.SubscriptionPlan;
            subscription.SubscriptionPeriod = command.SubscriptionPeriod;
            subscription.SubscriptionPrice = command.SubscriptionPrice;
            subscription.TotalAmount = command.SubscriptionPrice;
            subscription.PaidAmount = command.PaidAmount;
            subscription.RemainingAmount = command.SubscriptionPrice - command.PaidAmount;
            subscription.StartDate = command.StartDate;
            subscription.EndDate = CalculateEndDate(command.StartDate, command.SubscriptionPeriod);

            await subscriptionRepository.UpdateAsync(subscription);
            return MapSubscription(subscription);
        }

        public async Task<bool> DeactivateActiveSubscriptionAsync(Guid traineeId, string currentUserId)
        {
            var trainee = await traineeRepository.GetAll().FirstOrDefaultAsync(t => t.Id == traineeId);
            if (trainee is null)
            {
                return false;
            }

            await EnsureTraineeAccessAsync(trainee, currentUserId);

            var today = DateTime.Today;
            var activeSubscription = await subscriptionRepository.GetAll()
                .Where(s => s.TraineeId == traineeId && s.StartDate.Date <= today && s.EndDate.Date >= today)
                .OrderByDescending(s => s.EndDate)
                .FirstOrDefaultAsync();

            if (activeSubscription is null)
            {
                return false;
            }

            activeSubscription.EndDate = today.AddDays(-1);
            await subscriptionRepository.UpdateAsync(activeSubscription);
            return true;
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

            var installment = new Installment
            {
                SubscriptionId = subscription.Id,
                Amount = command.AmountPaid,
                CreationTime = DateTime.UtcNow
            };
            await installmentRepository.InsertAsync(installment);

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

        public async Task<Guid> AddExpenseAsync(CreateExpenseCommand command, string currentUserId)
        {
            var paidBy = command.PaidBy?.Trim();
            if (string.IsNullOrWhiteSpace(paidBy))
            {
                paidBy = (await userManager.FindByIdAsync(currentUserId))?.UserName ?? "غير محدد";
            }

            var expense = new Expense
            {
                Description = command.Description,
                Category = command.Category,
                PaidBy = paidBy,
                Notes = string.IsNullOrWhiteSpace(command.Notes) ? null : command.Notes.Trim(),
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
                Category = e.Category,
                PaidBy = e.PaidBy,
                Notes = e.Notes,
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

        public async Task<EarningsDashboardResult> GetEarningsDashboardAsync(int year, int month)
        {
            var subscriptions = await subscriptionRepository.GetAll()
                .Include(s => s.Trainee)
                .ToListAsync();
            var expenses = await expenseRepository.GetAll().ToListAsync();
            var installments = await installmentRepository.GetAll()
                .Include(i => i.Subscription)
                .ThenInclude(s => s.Trainee)
                .ToListAsync();

            // Unified list of payment events to support backwards compatibility
            var payments = new List<(DateTime Date, string Name, string Description, decimal Amount)>();

            // 1. Add all actual installments
            foreach (var inst in installments)
            {
                payments.Add((
                    inst.CreationTime,
                    inst.Subscription?.Trainee?.Name ?? "مشترك",
                    $"قسط - {inst.Subscription?.SubscriptionPlan} - {inst.Subscription?.SubscriptionPeriod}",
                    inst.Amount
                ));
            }

            // 2. Add historical subscriptions that don't have any installments mapped (backwards compatibility)
            var subscriptionIdsWithInstallments = installments.Select(i => i.SubscriptionId).ToHashSet();
            foreach (var sub in subscriptions)
            {
                if (sub.PaidAmount > 0 && !subscriptionIdsWithInstallments.Contains(sub.Id))
                {
                    payments.Add((
                        sub.StartDate,
                        sub.Trainee?.Name ?? "مشترك",
                        $"{sub.SubscriptionPlan} - {sub.SubscriptionPeriod}",
                        sub.PaidAmount
                    ));
                }
            }

            var totalExpenses = expenses.Sum(e => e.Amount);
            var monthlyExpenses = expenses
                .Where(e => e.ExpenseDate.Year == year && e.ExpenseDate.Month == month)
                .Sum(e => e.Amount);

            var totalEarningsBefore = payments.Sum(p => p.Amount);
            var monthlyEarningsBefore = payments
                .Where(p => p.Date.Year == year && p.Date.Month == month)
                .Sum(p => p.Amount);

            var totalEarningsAfter = totalEarningsBefore - totalExpenses;
            var monthlyEarningsAfter = monthlyEarningsBefore - monthlyExpenses;

            var currentMonthStart = new DateTime(year, month, 1);
            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var currentYearStart = new DateTime(year, 1, 1);
            var previousYearStart = currentYearStart.AddYears(-1);
            var previousYearEnd = currentYearStart.AddDays(-1);

            var previousMonthEarningsBefore = payments
                .Where(p => p.Date.Year == previousMonthStart.Year && p.Date.Month == previousMonthStart.Month)
                .Sum(p => p.Amount);
            var previousMonthExpenses = expenses
                .Where(e => e.ExpenseDate.Year == previousMonthStart.Year && e.ExpenseDate.Month == previousMonthStart.Month)
                .Sum(e => e.Amount);
            var previousMonthEarningsAfter = previousMonthEarningsBefore - previousMonthExpenses;

            var currentYearEarningsBefore = payments
                .Where(p => p.Date >= currentYearStart && p.Date <= currentMonthStart.AddMonths(1).AddTicks(-1))
                .Sum(p => p.Amount);
            var currentYearExpenses = expenses
                .Where(e => e.ExpenseDate >= currentYearStart && e.ExpenseDate <= currentMonthStart.AddMonths(1).AddTicks(-1))
                .Sum(e => e.Amount);
            var currentYearEarningsAfter = currentYearEarningsBefore - currentYearExpenses;

            var previousYearEarningsBefore = payments
                .Where(p => p.Date >= previousYearStart && p.Date <= previousYearEnd)
                .Sum(p => p.Amount);
            var previousYearExpenses = expenses
                .Where(e => e.ExpenseDate >= previousYearStart && e.ExpenseDate <= previousYearEnd)
                .Sum(e => e.Amount);
            var previousYearEarningsAfter = previousYearEarningsBefore - previousYearExpenses;

            var now = DateTime.UtcNow.Date;
            var activeSubscriptions = subscriptions.Count(s => s.EndDate.Date >= now);
            var newSubscriptionsThisMonth = subscriptions.Count(s => s.StartDate.Year == year && s.StartDate.Month == month);

            var monthlyTrend = Enumerable.Range(0, 5)
                .Select(offset => currentMonthStart.AddMonths(-offset))
                .OrderBy(d => d)
                .Select(d => {
                    var earningsVal = payments.Where(p => p.Date.Year == d.Year && p.Date.Month == d.Month).Sum(p => p.Amount);
                    var expensesVal = expenses.Where(e => e.ExpenseDate.Year == d.Year && e.ExpenseDate.Month == d.Month).Sum(e => e.Amount);
                    return new EarningsTrendPointResult
                    {
                        Year = d.Year,
                        Month = d.Month,
                        Amount = Math.Max(0, earningsVal - expensesVal)
                    };
                })
                .ToList();

            var subscriptionTransactions = payments
                .Select(p => new RecentFinancialTransactionResult
                {
                    Date = p.Date,
                    Name = p.Name,
                    Description = p.Description,
                    Amount = p.Amount,
                    Status = "Completed"
                });

            var expenseTransactions = expenses
                .Select(e => new RecentFinancialTransactionResult
                {
                    Date = e.ExpenseDate,
                    Name = string.IsNullOrWhiteSpace(e.PaidBy) ? "مصروف" : e.PaidBy,
                    Description = string.IsNullOrWhiteSpace(e.Category) ? e.Description : e.Category,
                    Amount = -e.Amount,
                    Status = "Expense"
                });

            var recentTransactions = subscriptionTransactions
                .Concat(expenseTransactions)
                .OrderByDescending(x => x.Date)
                .Take(5)
                .ToList();

            return new EarningsDashboardResult
            {
                TotalEarnings = totalEarningsAfter,
                MonthlyEarnings = monthlyEarningsAfter,
                TotalEarningsBeforeExpenses = totalEarningsBefore,
                MonthlyEarningsBeforeExpenses = monthlyEarningsBefore,
                ActiveSubscriptions = activeSubscriptions,
                NewSubscriptionsThisMonth = newSubscriptionsThisMonth,
                MonthOverMonthGrowthPercent = CalculateGrowthPercent(monthlyEarningsAfter, previousMonthEarningsAfter),
                YearOverYearGrowthPercent = CalculateGrowthPercent(currentYearEarningsAfter, previousYearEarningsAfter),
                MonthlyTrend = monthlyTrend,
                RecentTransactions = recentTransactions
            };
        }

        public async Task<List<GymPaymentTransactionResult>> GetTransactionsHistoryAsync(string currentUserId)
        {
            var isFemaleOnly = await IsFemaleOnlyUserAsync(currentUserId);
            if (isFemaleOnly)
            {
                return new List<GymPaymentTransactionResult>();
            }

            var subscriptions = await subscriptionRepository.GetAll()
                .Include(s => s.Trainee)
                .ToListAsync();

            var installments = await installmentRepository.GetAll()
                .Include(i => i.Subscription)
                .ThenInclude(s => s.Trainee)
                .ToListAsync();

            var expenses = await expenseRepository.GetAll().ToListAsync();

            var results = new List<GymPaymentTransactionResult>();

            // 1. Add installments
            foreach (var inst in installments)
            {
                results.Add(new GymPaymentTransactionResult
                {
                    Date = inst.CreationTime,
                    TraineeName = inst.Subscription?.Trainee?.Name ?? "مشترك",
                    Plan = inst.Subscription?.SubscriptionPlan.ToString() ?? "Basic",
                    Period = inst.Subscription?.SubscriptionPeriod.ToString() ?? "Monthly",
                    Amount = inst.Amount,
                    Type = "Installment"
                });
            }

            // 2. Add historical subscriptions
            var subscriptionIdsWithInstallments = installments.Select(i => i.SubscriptionId).ToHashSet();
            foreach (var sub in subscriptions)
            {
                if (sub.PaidAmount > 0 && !subscriptionIdsWithInstallments.Contains(sub.Id))
                {
                    results.Add(new GymPaymentTransactionResult
                    {
                        Date = sub.StartDate,
                        TraineeName = sub.Trainee?.Name ?? "مشترك",
                        Plan = sub.SubscriptionPlan.ToString(),
                        Period = sub.SubscriptionPeriod.ToString(),
                        Amount = sub.PaidAmount,
                        Type = "Subscription"
                    });
                }
            }

            // 3. Add expenses
            foreach (var exp in expenses)
            {
                results.Add(new GymPaymentTransactionResult
                {
                    Date = exp.ExpenseDate,
                    TraineeName = string.IsNullOrWhiteSpace(exp.PaidBy) ? "مصروف" : exp.PaidBy,
                    Plan = exp.Category ?? "مصروفات عامة",
                    Period = exp.Description ?? "مصاريف تشغيلية",
                    Amount = -exp.Amount,
                    Type = "Expense"
                });
            }

            return results.OrderByDescending(r => r.Date).ToList();
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
                SubscriptionPrice = subscription.SubscriptionPrice,
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

        private static decimal CalculateGrowthPercent(decimal current, decimal previous)
        {
            if (previous <= 0)
            {
                return current > 0 ? 100 : 0;
            }

            return ((current - previous) / previous) * 100;
        }
    }
}
