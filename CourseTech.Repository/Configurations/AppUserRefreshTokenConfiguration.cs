using CourseTech.Core.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseTech.Repository.Configurations
{
    public class AppUserRefreshTokenConfiguration : IEntityTypeConfiguration<AppUserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<AppUserRefreshToken> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Token).IsRequired().HasMaxLength(200);
        }
    }
}
