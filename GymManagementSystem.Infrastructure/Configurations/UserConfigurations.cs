using GymManagementSystem.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);
            
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
            
            
            builder.Property(u => u.Gender)
                .IsRequired();

            // builder.Property(u => u.Id)
            //    .HasColumnType("uniqueidentifier");

            // Configure relationship with UserRefreshTokens
            builder.HasMany(u => u.UserRefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
