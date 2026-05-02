using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    public class TraineeController : ControllerBase
    {
        [HttpGet("api/trainees")]
        [Authorize(Roles = "Admin")] // Only Admin and Trainer roles can access this endpoint
        public async Task<IActionResult> GetTrainees()
        {
            // Implement logic to retrieve trainees
            return Ok(new { message = "List of trainees" });
        }
    }
}
