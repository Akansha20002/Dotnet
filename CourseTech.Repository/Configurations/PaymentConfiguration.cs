using CourseTech.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseTech.Repository.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

            builder.Property(p => p.PaymentProvider)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.TransactionId)
                .HasMaxLength(100);

            builder.Property(p => p.PaymentDate)
                .IsRequired();

            builder.Property(p => p.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

            builder.Property(p => p.IsSuccessful)
                .IsRequired();

            builder.HasOne(p => p.AppUser)
               .WithMany()
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index Configurations
            builder.HasIndex(p => p.TransactionId)
                .IsUnique()
                .HasFilter("[TransactionId] IS NOT NULL");

            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.OrderId)
                .IsUnique();
        }
    }
}