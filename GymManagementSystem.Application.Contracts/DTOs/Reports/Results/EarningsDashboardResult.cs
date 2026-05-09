namespace GymManagementSystem.DTOs.Reports.Results
{
    public class EarningsDashboardResult
    {
        public decimal TotalEarnings { get; set; }
        public decimal MonthlyEarnings { get; set; }
        public int ActiveSubscriptions { get; set; }
        public int NewSubscriptionsThisMonth { get; set; }
        public decimal YearOverYearGrowthPercent { get; set; }
        public decimal MonthOverMonthGrowthPercent { get; set; }
        public List<EarningsTrendPointResult> MonthlyTrend { get; set; } = [];
        public List<RecentFinancialTransactionResult> RecentTransactions { get; set; } = [];
    }

    public class EarningsTrendPointResult
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
    }

    public class RecentFinancialTransactionResult
    {
        public DateTime Date { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
