using GymManagementSystem.Enums;
using Microsoft.AspNetCore.Http;

namespace GymManagementSystem.DTOs.Trainee.Commands
{
    public class UpdateTraineeCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public enGender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;
        public IFormFile? Photo { get; set; }
    }
}
