using CourseTech.Core.DTOs.Payment;
using CourseTech.Shared;

namespace CourseTech.Core.Services
{
    public interface IPaymentService
    {
        Task<ServiceResult<PaymentDTO>> ProcessPaymentAsync(PaymentRequestDTO paymentRequest);
        Task<ServiceResult<List<PaymentDTO>>> GetPaymentsByUserAsync(Guid userId);
        Task<ServiceResult<PaymentDTO>> GetPaymentByIdAsync(Guid paymentId);
    }
}