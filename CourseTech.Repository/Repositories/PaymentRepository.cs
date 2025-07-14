using CourseTech.Core.Models;
using CourseTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseTech.Repository.Repositories
{
    public class PaymentRepository(AppDbContext context) : IPaymentRepository
    {
        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await context.Payments
               .Include(p => p.AppUser)
               .Include(p => p.Order)
                   .ThenInclude(o => o.OrderItems)
               .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            return await context.Payments
                .Include(p => p.AppUser)
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<List<Payment>> GetByUserIdAsync(Guid userId)
        {
            return await context.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.OrderItems)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task CreateAsync(Payment payment)
        {
            await context.Payments.AddAsync(payment);
        }
    }
}