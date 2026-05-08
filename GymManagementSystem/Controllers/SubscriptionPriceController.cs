using GymManagementSystem.DTOs.SubscriptionPrice.Commands;
using GymManagementSystem.DTOs.SubscriptionPrice.Queries;
using GymManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    [ApiController]
    [Route("api/subscription-prices")]
    [Authorize]
    public class SubscriptionPriceController(ISubscriptionPriceService subscriptionPriceService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetSubscriptionPrices([FromQuery] GetAllSubscriptionPricesQuery query)
        {
            var result = await subscriptionPriceService.GetAllAsync(query);
            return Ok(result);
        }

        [HttpGet("{subscriptionPriceId:guid}")]
        public async Task<IActionResult> GetSubscriptionPrice(Guid subscriptionPriceId)
        {
            var result = await subscriptionPriceService.GetByIdAsync(new GetSubscriptionPriceQuery
            {
                SubscriptionPriceId = subscriptionPriceId
            });

            return result is null ? NotFound(new { message = "Subscription price not found." }) : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubscriptionPrice([FromBody] CreateSubscriptionPriceCommand command)
        {
            try
            {
                var subscriptionPriceId = await subscriptionPriceService.CreateAsync(command);
                return Ok(new { subscriptionPriceId, message = "Subscription price created successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{subscriptionPriceId:guid}")]
        public async Task<IActionResult> UpdateSubscriptionPrice(Guid subscriptionPriceId, [FromBody] UpdateSubscriptionPriceCommand command)
        {
            try
            {
                var updated = await subscriptionPriceService.UpdateAsync(subscriptionPriceId, command);
                return updated ? Ok(new { message = "Subscription price updated successfully." }) : NotFound(new { message = "Subscription price not found." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{subscriptionPriceId:guid}")]
        public async Task<IActionResult> DeleteSubscriptionPrice(Guid subscriptionPriceId)
        {
            var deleted = await subscriptionPriceService.DeleteAsync(subscriptionPriceId);
            return deleted ? Ok(new { message = "Subscription price deleted successfully." }) : NotFound(new { message = "Subscription price not found." });
        }
    }
}
