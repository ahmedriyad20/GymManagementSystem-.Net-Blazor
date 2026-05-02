using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DTOs.User.Commands
{
    public class RefreshTokenCommand
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
