using GymManagementSystem.Enums;

namespace GymManagementSystem.DTOs.Trainee.Commands
{
    public class CreateTraineeCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public enGender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
