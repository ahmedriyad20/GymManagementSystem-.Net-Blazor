using GymManagementSystem.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.Configurations
{
    public class UserRefreshTokensConfigurations : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.AccessToken)
                .IsRequired(false);

            builder.Property(x => x.RefreshToken)
                .IsRequired(false);

            builder.Property(x => x.JwtId)
                .IsRequired(false);

            // Proper relationship with User
            builder.HasOne(x => x.User)
                .WithMany(x => x.UserRefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
