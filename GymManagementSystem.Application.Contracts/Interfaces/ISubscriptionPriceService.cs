using GymManagementSystem.DTOs.SubscriptionPrice.Commands;
using GymManagementSystem.DTOs.SubscriptionPrice.Queries;
using GymManagementSystem.DTOs.SubscriptionPrice.Results;

namespace GymManagementSystem.Interfaces
{
    public interface ISubscriptionPriceService
    {
        Task<List<GetAllSubscriptionPricesResult>> GetAllAsync(GetAllSubscriptionPricesQuery query);
        Task<GetSubscriptionPriceResult?> GetByIdAsync(GetSubscriptionPriceQuery query);
        Task<Guid> CreateAsync(CreateSubscriptionPriceCommand command);
        Task<bool> UpdateAsync(Guid subscriptionPriceId, UpdateSubscriptionPriceCommand command);
        Task<bool> DeleteAsync(Guid subscriptionPriceId);
    }
}
