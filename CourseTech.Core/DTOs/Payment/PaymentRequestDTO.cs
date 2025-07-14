namespace CourseTech.Core.DTOs.Payment
{
    public record PaymentRequestDTO
    (
        Guid OrderId,
        string CardHolderName,
        string CardNumber,
        string ExpiryMonth,
        string ExpiryYear,
        string Cvv,
        decimal TotalAmount,
        DateTime PaymentDate
    );
}