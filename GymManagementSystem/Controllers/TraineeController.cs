using GymManagementSystem.DTOs.Trainee.Commands;
using GymManagementSystem.DTOs.Trainee.Queries;
using GymManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymManagementSystem.Controllers
{
    [ApiController]
    [Route("api/trainees")]
    [Authorize]
    public class TraineeController(ITraineeService traineeService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTrainees([FromQuery] GetAllTraineesQuery query)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var result = await traineeService.GetAllAsync(query, userId);
            return Ok(result);
        }

        [HttpGet("{traineeId:guid}")]
        public async Task<IActionResult> GetTrainee(Guid traineeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            var result = await traineeService.GetByIdAsync(traineeId, userId);
            return result is null ? NotFound(new { message = "Trainee not found." }) : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTrainee([FromBody] CreateTraineeCommand command)
        {
            var traineeId = await traineeService.CreateAsync(command);
            return Ok(new { traineeId, message = "Trainee created successfully." });
        }

        [HttpPut("{traineeId:guid}")]
        public async Task<IActionResult> UpdateTrainee(Guid traineeId, [FromBody] UpdateTraineeCommand command)
        {
            var updated = await traineeService.UpdateAsync(traineeId, command);
            return updated ? Ok(new { message = "Trainee updated successfully." }) : NotFound(new { message = "Trainee not found." });
        }

        [HttpDelete("{traineeId:guid}")]
        public async Task<IActionResult> DeleteTrainee(Guid traineeId)
        {
            var deleted = await traineeService.DeleteAsync(traineeId);
            return deleted ? Ok(new { message = "Trainee deleted successfully." }) : NotFound(new { message = "Trainee not found." });
        }
    }
}
