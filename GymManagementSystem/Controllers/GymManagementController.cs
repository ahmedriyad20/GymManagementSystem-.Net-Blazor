using GymManagementSystem.DTOs.Attendance.Commands;
using GymManagementSystem.DTOs.Expense.Commands;
using GymManagementSystem.DTOs.Subscription.Commands;
using GymManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymManagementSystem.Controllers
{
    [ApiController]
    [Route("api/gym")]
    [Authorize]
    public class GymManagementController(IGymManagementService gymManagementService) : ControllerBase
    {
        [HttpPost("subscriptions")]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            try
            {
                var result = await gymManagementService.CreateSubscriptionAsync(command, userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("subscriptions/{subscriptionId:guid}/installments")]
        public async Task<IActionResult> AddInstallment(Guid subscriptionId, [FromBody] AddInstallmentCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            try
            {
                var result = await gymManagementService.AddInstallmentAsync(subscriptionId, command, userId);
                return result is null ? NotFound(new { message = "Subscription not found." }) : Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("attendance")]
        public async Task<IActionResult> AddAttendance([FromBody] CreateAttendanceSessionCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            try
            {
                var created = await gymManagementService.AddAttendanceSessionAsync(command, userId);
                return created ? Ok(new { message = "Attendance added successfully." }) : NotFound(new { message = "Subscription or trainee not found." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("expenses")]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var expenseId = await gymManagementService.AddExpenseAsync(command, userId);
            return Ok(new { expenseId, message = "Expense added successfully." });
        }

        [HttpGet("expenses")]
        public async Task<IActionResult> GetExpenses([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var result = await gymManagementService.GetExpensesAsync(fromDate, toDate);
            return Ok(result);
        }

        [HttpGet("reports/expiring-3-days")]
        public async Task<IActionResult> GetExpiringIn3Days()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var result = await gymManagementService.GetExpiringSubscriptionsAsync(3, userId);
            return Ok(result);
        }

        [HttpGet("reports/expiring-7-days")]
        public async Task<IActionResult> GetExpiringIn7Days()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var result = await gymManagementService.GetExpiringSubscriptionsAsync(7, userId);
            return Ok(result);
        }

        [HttpGet("reports/unpaid-installments")]
        public async Task<IActionResult> GetUnpaidInstallments()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var result = await gymManagementService.GetUnpaidInstallmentsAsync(userId);
            return Ok(result);
        }

        [HttpGet("reports/earnings")]
        public async Task<IActionResult> GetEarnings([FromQuery] int year, [FromQuery] int month)
        {
            var result = await gymManagementService.GetEarningsSummaryAsync(year, month);
            return Ok(result);
        }

        [HttpGet("reports/earnings/dashboard")]
        public async Task<IActionResult> GetEarningsDashboard([FromQuery] int year, [FromQuery] int month)
        {
            var result = await gymManagementService.GetEarningsDashboardAsync(year, month);
            return Ok(result);
        }
    }
}
