using GymManagementSystem.DTOs.Trainee.Commands;
using GymManagementSystem.DTOs.Trainee.Queries;
using GymManagementSystem.DTOs.Trainee.Results;

namespace GymManagementSystem.Interfaces
{
    public interface ITraineeService
    {
        Task<List<GetAllTraineesResult>> GetAllAsync(GetAllTraineesQuery query, string currentUserId);
        Task<GetTraineeResult?> GetByIdAsync(Guid traineeId, string currentUserId);
        Task<Guid> CreateAsync(CreateTraineeCommand command);
        Task<bool> UpdateAsync(Guid traineeId, UpdateTraineeCommand command);
        Task<bool> DeleteAsync(Guid traineeId);
    }
}
