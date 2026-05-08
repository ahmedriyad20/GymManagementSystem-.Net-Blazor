using GymManagementSystem.Configurations;
using GymManagementSystem.Entities;
using GymManagementSystem.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;


namespace GymManagementSystem.EntityFrameworkCore;


public class GymManagementSystemDbContext : IdentityDbContext<User>
{
    public DbSet<Trainee> Trainees { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<AttendanceSession> AttendanceSessions { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<SubscriptionPrice> SubscriptionPrices { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    public GymManagementSystemDbContext(DbContextOptions<GymManagementSystemDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all your entity configurations here
        //builder.ApplyConfiguration(new TraineeConfiguration());
        //builder.ApplyConfiguration(new SubscriptionConfiguration());
        //builder.ApplyConfiguration(new UserRefreshTokensConfigurations());
        //builder.ApplyConfiguration(new UserConfiguration());

        // Apply configuration classes from this assembly (IEntityTypeConfiguration implementations)
        builder.ApplyConfigurationsFromAssembly(typeof(GymManagementSystemDbContext).Assembly);
    }
}
