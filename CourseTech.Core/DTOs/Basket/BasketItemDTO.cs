namespace CourseTech.Core.DTOs.Basket
{
    public record BasketItemDTO(
        Guid BasketId,
        Guid CourseId,
        string CourseTitle,
        string InstructorName,
        string ImageUrl,
        decimal Price
    );
}