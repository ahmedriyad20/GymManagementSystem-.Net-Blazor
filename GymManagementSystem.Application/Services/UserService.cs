using GymManagementSystem.DTOs.User.Commands;
using GymManagementSystem.DTOs.User.Results;
using GymManagementSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.Services
{
    public class UserService : IUserService
    {
        public Task CreateAsync(RegisterUserCommand command)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<GetUserDetailsResult> GetByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid userId, UpdateUserCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
