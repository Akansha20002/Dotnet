namespace CourseTech.Core.DTOs.Basket
{
    public record BasketDTO(
        Guid Id,
        Guid UserId,
        List<BasketItemDTO> Items,
        string Status,
        decimal TotalPrice
    );
}