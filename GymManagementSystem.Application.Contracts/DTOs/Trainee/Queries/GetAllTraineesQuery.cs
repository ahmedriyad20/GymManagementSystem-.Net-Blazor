using GymManagementSystem.Enums;

namespace GymManagementSystem.DTOs.Trainee.Queries
{
    public class GetAllTraineesQuery
    {
        public enGender? Gender { get; set; }
        public string? SearchText { get; set; }
        public bool Detailed { get; set; }
    }
}
