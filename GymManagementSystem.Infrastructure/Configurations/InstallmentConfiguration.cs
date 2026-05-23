using GymManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Configurations
{
    public class InstallmentConfiguration : IEntityTypeConfiguration<Installment>
    {
        public void Configure(EntityTypeBuilder<Installment> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Amount).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasOne(i => i.Subscription)
                .WithMany(s => s.Installments)
                .HasForeignKey(i => i.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
