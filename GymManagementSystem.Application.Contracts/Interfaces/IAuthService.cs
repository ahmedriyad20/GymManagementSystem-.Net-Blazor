using GymManagementSystem.DTOs.User.Commands;
using GymManagementSystem.DTOs.User.Results;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAdminAsync(RegisterUserCommand command);
        Task<AuthResult> LoginAsync(LoginCommand loginDto, ClaimsPrincipal user);
        Task<AuthResult> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<bool> RevokeTokenAsync(string userId);
    }
}
