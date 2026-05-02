using GymManagementSystem.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DTOs.User.Commands
{
    public class RegisterUserCommand
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public enGender Gender { get; set; }
    }
}
