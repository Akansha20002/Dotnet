using AutoMapper;
using CourseTech.Core.DTOs.Payment;
using CourseTech.Core.Models;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Shared;

namespace CourseTech.Service.Services
{
    public class PaymentService(IUnitOfWork unitOfWork, IMapper mapper) : IPaymentService
    {
        public async Task<ServiceResult<PaymentDTO>> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await unitOfWork.Payment.GetByIdAsync(paymentId);
            if (payment == null)
                return ServiceResult<PaymentDTO>.Fail("Payment not found.");
            var paymentDTO = mapper.Map<PaymentDTO>(payment);
            return ServiceResult<PaymentDTO>.Success(paymentDTO);
        }

        public async Task<ServiceResult<List<PaymentDTO>>> GetPaymentsByUserAsync(Guid userId)
        {
            var payments = await unitOfWork.Payment.GetByUserIdAsync(userId);
            if (payments == null)
                return ServiceResult<List<PaymentDTO>>.Fail("Payments not found.");

            var paymentDTOs = mapper.Map<List<PaymentDTO>>(payments);
            return ServiceResult<List<PaymentDTO>>.Success(paymentDTOs);
        }

        // eğer order controllerda order complete succces dönerse payment service çağrılır ve payment oluşturulur
        public async Task<ServiceResult<PaymentDTO>> ProcessPaymentAsync(PaymentRequestDTO paymentRequest)
        {
            var order = await unitOfWork.Order.GetOrderByIdWithIncludesAsync(paymentRequest.OrderId);
            if (order == null)
                return ServiceResult<PaymentDTO>.Fail("Order not found.");

            var transactionId = Guid.NewGuid().ToString();

            var payment = new Payment(
                userId: order.UserId,
                orderId: order.Id,
                order: order,
                totalAmount: order.TotalPrice,
                paymentProvider: "FakeProvider"
            );

            await unitOfWork.Payment.CreateAsync(payment);
            order.MarkAsCompleted();
            payment.MarkAsSuccessful(transactionId);
            await unitOfWork.SaveChangesAsync();

            var paymentDTO = mapper.Map<PaymentDTO>(payment);
            return ServiceResult<PaymentDTO>.Success(paymentDTO);
        }
    }
}