using CourseTech.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseTech.Repository.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(100)
                .IsRequired();

            // AppUser - Courses (One-to-Many)
            builder.HasMany(a => a.Courses)
                   .WithOne(c => c.Instructor)
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Restrict);

            // AppUser - Enrollment (One-to-Many)
            builder.HasMany(a => a.Enrollments)
                   .WithOne(e => e.AppUser)
                   .HasForeignKey(e => e.AppUserId)
                   .OnDelete(DeleteBehavior.Restrict); 

            // AppUser - Baskets (One-to-Many)
            builder.HasMany(a => a.Baskets)
                   .WithOne(b => b.AppUser)
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // AppUser - Payments (One-to-Many)
            builder.HasMany(a => a.Payments)
                   .WithOne(p => p.AppUser)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}