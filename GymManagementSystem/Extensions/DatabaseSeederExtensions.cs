
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GymManagementSystem.EntityFrameworkCore;
using GymManagementSystem.Entities.Users;
using GymManagementSystem.Seeder;

namespace GymManagementSystem.Extensions
{
    public static class DatabaseSeederExtensions
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    var context = services.GetRequiredService<GymManagementSystemDbContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Check if there are any pending migrations first (safe & fast)
                    var pending = await context.Database.GetPendingMigrationsAsync();
                    if (pending.Any())
                    {
                        logger.LogInformation("Applying {Count} pending migrations...", pending.Count());
                        await context.Database.MigrateAsync();
                        logger.LogInformation("Database migrated successfully.");
                    }
                    else
                    {
                        logger.LogInformation("No pending migrations.");
                    }

                    // Seed Identity Roles
                    await SeedRolesAsync(roleManager, logger);

                    // Pass the ROOT service provider (app.Services), not the scoped one
                    await DbSeeder.SeedAsync(app.Services);
                    logger.LogInformation("Database seeding finished.");
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                }
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            string[] roles = { "Admin", "Receptionist" };
            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation($"Role '{role}' created.");
                }
            }
        }
    }
}