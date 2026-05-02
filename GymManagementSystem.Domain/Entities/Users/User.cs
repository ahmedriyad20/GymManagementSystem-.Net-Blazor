using System;
using System.Collections.Generic;
using System.Text;
using GymManagementSystem.Enums;
using Microsoft.AspNetCore.Identity;

namespace GymManagementSystem.Entities.Users
{
    public class User : IdentityUser
    {
        //public string Password { get; set; } = string.Empty;
        public enGender Gender { get; set; }

        public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; set; } = new HashSet<UserRefreshToken>();
    }
}
