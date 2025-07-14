namespace CourseTech.Core.DTOs.Order
{
    public record OrderItemDTO
    (
        Guid CourseId,
        string Title,
        decimal Price,
        string ImageUrl
    );
}