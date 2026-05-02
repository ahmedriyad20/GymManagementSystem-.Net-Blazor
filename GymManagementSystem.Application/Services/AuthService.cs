using GymManagementSystem.DTOs.User.Commands;
using GymManagementSystem.DTOs.User.Results;
using GymManagementSystem.Entities.Users;
using GymManagementSystem.Enums;
using GymManagementSystem.Interfaces;
using HireAI.Infrastructure.GenaricBasies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace GymManagementSystem.Services
{
    public class AuthService(
        UserManager<User> _userManager,
        IRepository<UserRefreshToken> userRefreshTokenRepository,
        IConfiguration _config) : IAuthService
    {
        public async Task<AuthResult> RegisterAdminAsync(RegisterUserCommand command)
        {
            try
            {
                // Validate if email already exists
                var existingEmail = await _userManager.FindByEmailAsync(command.UserName);
                if (existingEmail != null)
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "Email already registered"
                    };
                }

                // Create ApplicationUser (Identity) - Use email as username
                var ApplicationUser = new User
                {
                    UserName = command.UserName, // Set email as username
                    Email = command.UserName,
                    Gender = command.Gender,
                };

                var result = await _userManager.CreateAsync(ApplicationUser, command.Password);

                if (!result.Succeeded)
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "Registration failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    };
                }

                // Assign Admin role
                await _userManager.AddToRoleAsync(ApplicationUser, "Admin");

                return new AuthResult
                {
                    IsAuthenticated = true,
                    IdentityUserId = ApplicationUser.Id,
                    UserId = ApplicationUser.Id,
                    UserRole = "Admin",
                    Message = "Admin registered successfully"
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsAuthenticated = false,
                    Message = "An error occurred during registration",
                    Errors = new List<string> { ex.Message }
                };
            }
        }



        public async Task<AuthResult> LoginAsync(LoginCommand loginDto, ClaimsPrincipal user)
        {
            try
            {
                // Find user by email instead of username
                var ApplicationUser = await _userManager.FindByEmailAsync(loginDto.UserName);

                if (ApplicationUser == null)
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "Invalid email or password"
                    };
                }

                var isPasswordCorrect = await _userManager.CheckPasswordAsync(ApplicationUser, loginDto.Password);

                if (!isPasswordCorrect)
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "Invalid email or password"
                    };
                }

                var roles = await _userManager.GetRolesAsync(ApplicationUser);
                var token = GenerateJwtToken(ApplicationUser, roles);
                var refreshToken = GenerateRefreshToken();


                // Save refresh token to database - FIXED: Set User navigation property
                var userRefreshToken = new UserRefreshToken
                {
                    UserId = ApplicationUser.Id,
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    JwtId = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.Now,
                    ExpiryDate = DateTime.Now.AddDays(7),
                    IsRevoked = false // ✅ Explicitly set this
                };

                await userRefreshTokenRepository.InsertAsync(userRefreshToken);

                // Determine user type and ID first
                
                string? userRole = null;

                if (ApplicationUser.Gender == enGender.Male)
                {
                    userRole = "Admin";
                }
                else if (ApplicationUser.Gender == enGender.Female)
                {
                    userRole = "Receptionist";
                }
                else
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "User profile not found"
                    };
                }

                return new AuthResult
                {
                    IsAuthenticated = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresOn = DateTime.Now.AddMinutes(30),
                    IdentityUserId = ApplicationUser.Id, // Identity framework ID
                    UserId = ApplicationUser.Id, // Applicant/HR ID
                    UserRole = userRole, // roles.Contains("Applicant") ? "Applicant" : "HR",
                    Message = "Login successful"
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsAuthenticated = false,
                    Message = "An error occurred during login",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<AuthResult> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(accessToken);

                if (principal == null)
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "Invalid token"
                    };
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "Invalid token claims"
                    };
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "User not found"
                    };
                }

                // Validate refresh token from database - Query directly instead of using navigation property
                var userRefreshTokensQueryable = userRefreshTokenRepository.GetAll();
                var storedRefreshToken = await userRefreshTokensQueryable
                    .FirstOrDefaultAsync(t =>
                        t.UserId == userId &&
                        t.RefreshToken == refreshToken &&
                        !t.IsRevoked &&
                        t.ExpiryDate > DateTime.Now);

                if (storedRefreshToken == null)
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "Invalid or expired refresh token"
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);
                var newAccessToken = GenerateJwtToken(user, roles);
                var newRefreshToken = GenerateRefreshToken();

                // Update refresh token
                storedRefreshToken.RefreshToken = newRefreshToken;
                storedRefreshToken.AccessToken = newAccessToken;
                storedRefreshToken.CreatedDate = DateTime.Now;

                await userRefreshTokenRepository.UpdateAsync(storedRefreshToken);
                //_dbContext.Set<UserRefreshToken>().Update(storedRefreshToken);
                //await _dbContext.SaveChangesAsync();

                // Determine user type and ID 
                int? userSpecificId = null;
                string? userRole = null;

                if (user.Gender == enGender.Male)
                {
                    //userSpecificId = user.ApplicantId;
                    userRole = "Admin";
                }
                else if (user.Gender == enGender.Female)
                {
                    //userSpecificId = user.HRId;
                    userRole = "Receptionist";
                }
                else
                {
                    return new AuthResult
                    {
                        IsAuthenticated = false,
                        Message = "User profile not found"
                    };
                }

                return new AuthResult
                {
                    IsAuthenticated = true,
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresOn = DateTime.Now.AddMinutes(30),
                    IdentityUserId = user.Id,
                    UserId = user.Id,
                    UserRole = userRole,
                    Message = "Token refreshed successfully"
                };
            }
            catch (Exception ex)
            {
                return new AuthResult
                {
                    IsAuthenticated = false,
                    Message = "Token refresh failed",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<bool> RevokeTokenAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                    return false;

                // Query tokens directly from database
                var userTokensQueryable = userRefreshTokenRepository.GetAll();
                var userTokens = await userTokensQueryable.ToListAsync();
                //var userTokens = await _dbContext.Set<UserRefreshToken>()
                //    .Where(t => t.UserId == userId)
                //    .ToListAsync();

                // Mark all user tokens as revoked
                foreach (var token in userTokens)
                {
                    token.IsRevoked = true;
                    await userRefreshTokenRepository.UpdateAsync(token);
                }

                //await userRefreshTokenRepository.UpdateManyAsync(userTokens);
                //_dbContext.Set<UserRefreshToken>().UpdateRange(userTokens);
                //await _dbContext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty), // Use UserName in Name claim
                new Claim(ClaimTypes.Email, user.UserName ?? string.Empty)
            };

            //Add all User Roles to the UserClaims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Add the Token Generated id change (JWT Predefind Claims ) to generate new token every login
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            //Now it's time for the Signature part in the token
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecurityKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Design Token
            var token = new JwtSecurityToken(
                issuer: _config["JWT:IssuerIP"],
                audience: _config["JWT:AudienceIP"], //Blazor Localhost
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["JWT:ExpireInMinutes"])),
                signingCredentials: signingCredentials
            );

            //Generate Token response
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SecurityKey"])),
                    ValidateLifetime = false // Allow expired tokens
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private async Task<bool> IsAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.Contains("Admin");
        }
    }
}
