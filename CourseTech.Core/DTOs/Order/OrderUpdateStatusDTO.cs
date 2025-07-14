namespace CourseTech.Core.DTOs.Order
{
    public record OrderUpdateStatusDTO
    (
        Guid OrderId,
        string Status
    );
}