using GymManagementSystem.DTOs.Trainee.Commands;
using GymManagementSystem.DTOs.Trainee.Results;
using GymManagementSystem.Entities;
using GymManagementSystem.Interfaces;
using HireAI.Infrastructure.GenaricBasies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace GymManagementSystem.Services
{
    public class TraineeService(IRepository<Trainee> traineeRepository) : ITraineeService
    {
        public Task<GetAllTraineesResult> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GetTraineeResult> GetByIdAsync(Guid traineeId)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(CreateTraineeCommand command)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid traineeId, UpdateTraineeCommand command)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid traineeId)
        {
            throw new NotImplementedException();
        }
    }
}
