
using GymManagementSystem.Interfaces;
using GymManagementSystem.Services;


namespace GymManagementSystem.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITraineeService, TraineeService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}