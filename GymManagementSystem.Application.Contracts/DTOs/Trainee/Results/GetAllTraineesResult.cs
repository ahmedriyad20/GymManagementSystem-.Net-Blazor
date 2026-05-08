namespace GymManagementSystem.DTOs.Trainee.Results
{
    public class GetAllTraineesResult
    {
        public Guid TraineeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
        public DateTime? CurrentSubscriptionEndDate { get; set; }
        public decimal? RemainingAmount { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
