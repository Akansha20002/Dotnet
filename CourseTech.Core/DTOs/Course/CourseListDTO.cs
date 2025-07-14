namespace CourseTech.Core.DTOs.Course
{
    public record CourseListDTO
    (
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        string ImageUrl,
        string CategoryName,
        string InstructorName,
        TimeSpan Duration,
        string CourseLevel,
        bool IsPublished
    );
}