namespace CourseTech.Core.DTOs.Order
{
    public record OrderDTO
    (
        Guid Id,
        Guid UserId,
        string Email,
        string FullName,
        decimal TotalPrice,
        string Status,
        List<OrderItemDTO> OrderItems,
        DateTime CreatedAt
    );
}