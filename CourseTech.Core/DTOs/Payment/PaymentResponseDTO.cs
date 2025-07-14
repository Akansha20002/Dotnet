namespace CourseTech.Core.DTOs.Payment
{
    public record PaymentResponseDTO
    (
        Guid Id,
        string Message,
        bool IsSuccessful
    );
}