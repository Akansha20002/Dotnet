using CourseTech.Core.DTOs.Course;
using CourseTech.Shared;

namespace CourseTech.Core.Services
{
    public interface ICourseService
    {
        Task<ServiceResult<CourseDTO>> GetByIdAsync(Guid courseId);
        Task<ServiceResult<IEnumerable<CourseDTO>>> GetAllAsync();
        Task<ServiceResult<CourseDTO>> CreateAsync(CourseCreateDTO courseDto);
        Task<ServiceResult<CourseDTO>> UpdateAsync(CourseUpdateDTO courseDto);
        Task<ServiceResult> SoftDeleteAsync(Guid courseId);

        Task<ServiceResult<IEnumerable<CourseSummaryDTO>>> GetAllCoursesSummariesForCardAsync();
        Task<ServiceResult<IEnumerable<CourseDTO>>> GetPublishedCoursesAsync();
        Task<ServiceResult<IEnumerable<CourseListDTO>>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<ServiceResult<IEnumerable<CourseSummaryDTO>>> GetCoursesByCategoryAsync(Guid categoryId);
        Task<ServiceResult<CourseDTO>> GetCourseWithDetailsAsync(Guid courseId);

        Task<ServiceResult> PublishCourseAsync(Guid courseId);
        Task<ServiceResult> UnpublishCourseAsync(Guid courseId);
    }
}