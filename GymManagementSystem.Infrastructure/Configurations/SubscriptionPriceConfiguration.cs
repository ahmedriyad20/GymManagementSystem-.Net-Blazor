using GymManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Configurations
{
    public class SubscriptionPriceConfiguration : IEntityTypeConfiguration<SubscriptionPrice>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPrice> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.SubscriptionPlan).IsRequired();
            builder.Property(p => p.SubscriptionPeriod).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasIndex(p => new { p.SubscriptionPlan, p.SubscriptionPeriod }).IsUnique();
        }
    }
}
