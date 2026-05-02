using GymManagementSystem.DTOs.Trainee.Commands;
using GymManagementSystem.DTOs.Trainee.Results;
using GymManagementSystem.DTOs.User.Commands;
using GymManagementSystem.DTOs.User.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Interfaces
{
    public interface IUserService
    {
        Task<GetUserDetailsResult> GetByIdAsync(Guid userId);
        Task CreateAsync(RegisterUserCommand command);
        Task UpdateAsync(Guid userId, UpdateUserCommand command);
        Task DeleteAsync(Guid userId);
    }
}
