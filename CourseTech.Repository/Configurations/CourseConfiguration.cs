using CourseTech.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseTech.Repository.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasIndex(c => c.InstructorId);
            builder.HasIndex(c => c.CategoryId);

            builder.Property(x => x.Title)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.Level)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.Language)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.Price)
                .HasPrecision(18, 2);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.VideoUrl)
                .HasMaxLength(500);

            builder.Property(x => x.IsPublished)
                .HasDefaultValue(false);


            // Instructor - Course (One-to-Many)
            builder.HasOne(c => c.Instructor)
                   .WithMany(a => a.Courses)
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Course - Category (Many-to-One)
            builder.HasOne(c => c.Category)
                   .WithMany(cat => cat.Courses)
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
