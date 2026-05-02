

using HireAI.Infrastructure.GenaricBasies;

namespace GymManagementSystem.Extensions
{
    public static class RepositoryRegistrationExtensions
    {
        public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }
    }
}