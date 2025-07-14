namespace CourseTech.Core.DTOs.Course
{
    public record CourseCreateDTO
    (
        string Title, 
        string? Description, 
        string ImageUrl,
        string VideoUrl,
        string Level, 
        string Language, 
        decimal Price, 
        TimeSpan Duration, 
        Guid InstructorId, 
        Guid CategoryId
    );
}