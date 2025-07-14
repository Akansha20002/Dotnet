namespace CourseTech.Core.DTOs.Course
{
    public record CourseUpdateDTO
    (
        Guid Id, 
        string Title, 
        string? Description, 
        string ImageUrl, 
        string VideoUrl, 
        string Level, 
        string Language, 
        decimal Price, 
        TimeSpan Duration, 
        bool IsPublished, 
        DateTime? PublishedAt, 
        Guid InstructorId, 
        Guid CategoryId
    );
}