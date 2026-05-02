using GymManagementSystem.DTOs.Trainee.Commands;
using GymManagementSystem.DTOs.Trainee.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Interfaces
{
    public interface ITraineeService
    {
        Task<GetAllTraineesResult> GetAllAsync();
        Task<GetTraineeResult> GetByIdAsync(Guid traineeId);
        Task CreateAsync(CreateTraineeCommand command);
        Task UpdateAsync(Guid traineeId, UpdateTraineeCommand command);
        Task DeleteAsync(Guid traineeId);
    }
}
