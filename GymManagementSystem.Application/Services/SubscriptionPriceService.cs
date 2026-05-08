using GymManagementSystem.DTOs.SubscriptionPrice.Commands;
using GymManagementSystem.DTOs.SubscriptionPrice.Queries;
using GymManagementSystem.DTOs.SubscriptionPrice.Results;
using GymManagementSystem.Entities;
using GymManagementSystem.Interfaces;
using HireAI.Infrastructure.GenaricBasies;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Services
{
    public class SubscriptionPriceService(IRepository<SubscriptionPrice> subscriptionPriceRepository) : ISubscriptionPriceService
    {
        public async Task<List<GetAllSubscriptionPricesResult>> GetAllAsync(GetAllSubscriptionPricesQuery query)
        {
            var entities = await subscriptionPriceRepository.GetAll()
                .OrderBy(x => x.SubscriptionPlan)
                .ThenBy(x => x.SubscriptionPeriod)
                .ToListAsync();

            return entities.Select(x => new GetAllSubscriptionPricesResult
            {
                SubscriptionPriceId = x.Id,
                SubscriptionPlan = x.SubscriptionPlan.ToString(),
                SubscriptionPeriod = x.SubscriptionPeriod.ToString(),
                Price = x.Price
            }).ToList();
        }

        public async Task<GetSubscriptionPriceResult?> GetByIdAsync(GetSubscriptionPriceQuery query)
        {
            var entity = await subscriptionPriceRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == query.SubscriptionPriceId);

            if (entity is null)
            {
                return null;
            }

            return new GetSubscriptionPriceResult
            {
                SubscriptionPriceId = entity.Id,
                SubscriptionPlan = entity.SubscriptionPlan.ToString(),
                SubscriptionPeriod = entity.SubscriptionPeriod.ToString(),
                Price = entity.Price
            };
        }

        public async Task<Guid> CreateAsync(CreateSubscriptionPriceCommand command)
        {
            var exists = await subscriptionPriceRepository.GetAll().AnyAsync(x =>
                x.SubscriptionPlan == command.SubscriptionPlan &&
                x.SubscriptionPeriod == command.SubscriptionPeriod);

            if (exists)
            {
                throw new InvalidOperationException("A price already exists for the selected plan and period.");
            }

            var entity = new SubscriptionPrice
            {
                SubscriptionPlan = command.SubscriptionPlan,
                SubscriptionPeriod = command.SubscriptionPeriod,
                Price = command.Price
            };

            await subscriptionPriceRepository.InsertAsync(entity);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Guid subscriptionPriceId, UpdateSubscriptionPriceCommand command)
        {
            var entity = await subscriptionPriceRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == subscriptionPriceId);

            if (entity is null)
            {
                return false;
            }

            var duplicate = await subscriptionPriceRepository.GetAll().AnyAsync(x =>
                x.Id != subscriptionPriceId &&
                x.SubscriptionPlan == command.SubscriptionPlan &&
                x.SubscriptionPeriod == command.SubscriptionPeriod);

            if (duplicate)
            {
                throw new InvalidOperationException("Another price already uses this plan and period.");
            }

            entity.SubscriptionPlan = command.SubscriptionPlan;
            entity.SubscriptionPeriod = command.SubscriptionPeriod;
            entity.Price = command.Price;

            await subscriptionPriceRepository.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid subscriptionPriceId)
        {
            var entity = await subscriptionPriceRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == subscriptionPriceId);

            if (entity is null)
            {
                return false;
            }

            await subscriptionPriceRepository.DeleteAsync(entity);
            return true;
        }
    }
}
