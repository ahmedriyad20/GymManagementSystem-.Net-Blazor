using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DTOs.User.Results
{
    public class AuthResult
    {
        public bool IsAuthenticated { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public string? RefreshToken { get; set; }
        public string? Message { get; set; }
        public string? IdentityUserId { get; set; } // ASP.NET Identity User ID
        public string? UserId { get; set; } 
        public string? UserRole { get; set; } // "Admin" or "Receptionist"
        public List<string>? Errors { get; set; } = new();
    }
}
