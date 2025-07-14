namespace CourseTech.Core.DTOs.Order
{
    public record OrderSummaryDTO
    (
        Guid Id,
        decimal TotalPrice,
        string Status,
        DateTime CreatedAt
    );
}