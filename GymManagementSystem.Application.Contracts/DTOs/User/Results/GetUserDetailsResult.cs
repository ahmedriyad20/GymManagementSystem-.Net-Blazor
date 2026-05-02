using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DTOs.User.Results
{
    public class GetUserDetailsResult
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
