using GymManagementSystem.DTOs.User.Commands;
using GymManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.Controllers
{
    public class AuthController(IAuthService _authService) : ControllerBase
    {

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            var response = await _authService.RegisterAdminAsync(registerDto);
            return response.IsAuthenticated ? Ok(response) : BadRequest(response);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

            var response = await _authService.LoginAsync(loginDto, User);

            return response.IsAuthenticated ? Ok(response) : Unauthorized(response);
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand refreshTokenDto)
        {
            var response = await _authService.RefreshTokenAsync(refreshTokenDto.AccessToken, refreshTokenDto.RefreshToken);

            return response.IsAuthenticated ? Ok(response) : Unauthorized(response);
        }

        [HttpPost("Logout")]
        [Authorize] // Authenticated users only (any role)
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated" });

            var success = await _authService.RevokeTokenAsync(userId);

            return success ? Ok(new { message = "Logout successful" }) : BadRequest(new { message = "Logout failed" });
        }
    }
}
