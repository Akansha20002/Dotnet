using CourseTech.Shared.Enums;

namespace CourseTech.Core.DTOs.Payment
{
    public record PaymentHistoryDTO
    (
        Guid Id,
        Guid OrderId,
        string TransactionId,
        decimal TotalAmount,
        string PaymentProvider,
        bool IsSuccessful,
        DateTime PaymentDate,
        PaymentStatus Status
    );
}