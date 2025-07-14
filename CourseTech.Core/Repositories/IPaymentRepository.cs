using CourseTech.Core.Models;

namespace CourseTech.Core.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment?> GetByTransactionIdAsync(string transactionId);
        Task<List<Payment>> GetByUserIdAsync(Guid userId); //admin

        Task CreateAsync(Payment payment);
    }
}