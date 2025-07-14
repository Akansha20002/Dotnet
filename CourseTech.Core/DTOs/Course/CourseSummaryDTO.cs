namespace CourseTech.Core.DTOs.Course
{
    public record CourseSummaryDTO
    (
        Guid Id, 
        string Title, 
        decimal Price, 
        string ImageUrl, 
        string InstructorName, 
        string CourseLevel
    );
}