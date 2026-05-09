using GymManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Configurations
{
    public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Description).IsRequired().HasMaxLength(500);
            builder.Property(e => e.Category).IsRequired().HasMaxLength(120);
            builder.Property(e => e.PaidBy).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Notes).HasMaxLength(1000);
            builder.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(e => e.ExpenseDate).IsRequired();
        }
    }
}
