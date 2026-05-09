using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using GymManagementSystem.DTOs.Attendance.Commands;
using GymManagementSystem.DTOs.Expense.Commands;
using GymManagementSystem.DTOs.Expense.Results;
using GymManagementSystem.DTOs.Reports.Results;
using GymManagementSystem.DTOs.Subscription.Commands;
using GymManagementSystem.DTOs.Subscription.Results;
using GymManagementSystem.DTOs.SubscriptionPrice.Results;
using GymManagementSystem.DTOs.Trainee.Commands;
using GymManagementSystem.DTOs.Trainee.Results;
using GymManagementSystem.DTOs.User.Commands;
using GymManagementSystem.DTOs.User.Results;
using GymManagementSystem.Enums;

namespace GymManagementSystem.BlazorWASM.Services;

public class ApiClient(HttpClient httpClient)
{
    public async Task<AuthResult?> LoginAsync(LoginCommand command)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/Login", command);
        return await ReadAuthResultAsync(response);
    }

    public async Task LogoutAsync() => await httpClient.PostAsync("api/auth/Logout", null);

    public async Task<List<GetAllTraineesResult>> GetTraineesAsync(string? search, enGender? gender)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(search)) query.Add($"SearchText={Uri.EscapeDataString(search)}");
        if (gender.HasValue) query.Add($"Gender={(int)gender.Value}");

        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        return await httpClient.GetFromJsonAsync<List<GetAllTraineesResult>>($"api/trainees{suffix}") ?? [];
    }

    public async Task<GetTraineeResult?> GetTraineeAsync(Guid traineeId)
    {
        return await httpClient.GetFromJsonAsync<GetTraineeResult>($"api/trainees/{traineeId}");
    }

    public async Task<Guid> CreateTraineeAsync(CreateTraineeCommand command, IBrowserFile? photoFile)
    {
        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(command.Name), nameof(command.Name));
        formData.Add(new StringContent(command.Phone), nameof(command.Phone));
        formData.Add(new StringContent(((int)command.Gender).ToString()), nameof(command.Gender));
        formData.Add(new StringContent(command.DateOfBirth.ToString("O")), nameof(command.DateOfBirth));

        if (photoFile is not null)
        {
            var stream = photoFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024);
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(photoFile.ContentType);
            formData.Add(fileContent, nameof(command.Photo), photoFile.Name);
        }

        var response = await httpClient.PostAsync("api/trainees", formData);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<CreateTraineeResponse>();
        return payload?.TraineeId ?? Guid.Empty;
    }

    public async Task UpdateTraineeAsync(Guid traineeId, UpdateTraineeCommand command, IBrowserFile? photoFile)
    {
        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(command.Name), nameof(command.Name));
        formData.Add(new StringContent(command.Phone), nameof(command.Phone));
        formData.Add(new StringContent(((int)command.Gender).ToString()), nameof(command.Gender));
        formData.Add(new StringContent(command.DateOfBirth.ToString("O")), nameof(command.DateOfBirth));
        formData.Add(new StringContent(command.IsActive.ToString()), nameof(command.IsActive));

        if (photoFile is not null)
        {
            var stream = photoFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024);
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(photoFile.ContentType);
            formData.Add(fileContent, nameof(command.Photo), photoFile.Name);
        }

        var response = await httpClient.PutAsync($"api/trainees/{traineeId}", formData);
        response.EnsureSuccessStatusCode();
    }

    public async Task<SubscriptionResult?> CreateSubscriptionAsync(CreateSubscriptionCommand command)
    {
        var response = await httpClient.PostAsJsonAsync("api/gym/subscriptions", command);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SubscriptionResult>();
    }

    public async Task<SubscriptionResult?> AddInstallmentAsync(Guid subscriptionId, decimal amount)
    {
        var response = await httpClient.PostAsJsonAsync($"api/gym/subscriptions/{subscriptionId}/installments", new AddInstallmentCommand { AmountPaid = amount });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SubscriptionResult>();
    }

    public async Task AddAttendanceAsync(CreateAttendanceSessionCommand command)
    {
        var response = await httpClient.PostAsJsonAsync("api/gym/attendance", command);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ExpiringSubscriptionResult>> GetExpiringSubscriptionsByDaysAsync(int days)
    {
        var route = days <= 3 ? "api/gym/reports/expiring-3-days" : "api/gym/reports/expiring-7-days";
        var list = await httpClient.GetFromJsonAsync<List<ExpiringSubscriptionResult>>(route) ?? [];
        return list.Where(x => x.RemainingDays <= days).ToList();
    }

    public async Task<List<UnpaidInstallmentResult>> GetUnpaidInstallmentsAsync()
    {
        return await httpClient.GetFromJsonAsync<List<UnpaidInstallmentResult>>("api/gym/reports/unpaid-installments") ?? [];
    }

    public async Task<EarningsSummaryResult?> GetEarningsAsync(int year, int month)
    {
        return await httpClient.GetFromJsonAsync<EarningsSummaryResult>($"api/gym/reports/earnings?year={year}&month={month}");
    }

    public async Task<EarningsDashboardResult?> GetEarningsDashboardAsync(int year, int month)
    {
        return await httpClient.GetFromJsonAsync<EarningsDashboardResult>($"api/gym/reports/earnings/dashboard?year={year}&month={month}");
    }

    public async Task<List<ExpenseResult>> GetExpensesAsync(DateTime? fromDate, DateTime? toDate)
    {
        var query = new List<string>();
        if (fromDate.HasValue) query.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");
        if (toDate.HasValue) query.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");
        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        return await httpClient.GetFromJsonAsync<List<ExpenseResult>>($"api/gym/expenses{suffix}") ?? [];
    }

    public async Task CreateExpenseAsync(CreateExpenseCommand command)
    {
        var response = await httpClient.PostAsJsonAsync("api/gym/expenses", command);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<GetAllSubscriptionPricesResult>> GetSubscriptionPricesAsync()
    {
        return await httpClient.GetFromJsonAsync<List<GetAllSubscriptionPricesResult>>("api/subscription-prices") ?? [];
    }

    private class CreateTraineeResponse
    {
        public Guid TraineeId { get; set; }
    }

    private static async Task<AuthResult> ReadAuthResultAsync(HttpResponseMessage response)
    {
        var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
        var body = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrWhiteSpace(body) && contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<AuthResult>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed is not null)
                {
                    return parsed;
                }
            }
            catch
            {
                // fallback below
            }
        }

        return new AuthResult
        {
            IsAuthenticated = false,
            Message = string.IsNullOrWhiteSpace(body) ? $"HTTP {(int)response.StatusCode}" : body
        };
    }
}
