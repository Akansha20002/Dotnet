using CourseTech.Shared.Enums;

namespace CourseTech.Core.DTOs.Payment
{
    public record PaymentDTO
    (
        Guid Id,
        Guid UserId,
        Guid OrderId,
        string TransactionId,
        decimal TotalAmount,
        string PaymentProvider,
        bool IsSuccessful,
        DateTime PaymentDate,
        PaymentStatus Status
    );
}