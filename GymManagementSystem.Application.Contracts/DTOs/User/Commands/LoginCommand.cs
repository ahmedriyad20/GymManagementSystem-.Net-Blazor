using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem.DTOs.User.Commands
{
    public class LoginCommand
    {
        [Required(ErrorMessage = "UserName is required")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = default!;
    }
}
