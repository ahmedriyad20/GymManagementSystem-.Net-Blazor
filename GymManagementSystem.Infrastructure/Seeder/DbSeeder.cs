
using GymManagementSystem.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace GymManagementSystem.Seeder
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, bool forceReseed = false)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymManagementSystemDbContext>();

            // Apply migrations
            try
            {
                await context.Database.MigrateAsync();
            }
            catch
            {
                // Ignore migration errors
            }
        }
    }
}
