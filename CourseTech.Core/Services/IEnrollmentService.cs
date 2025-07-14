using CourseTech.Core.DTOs.AppUser;
using CourseTech.Core.DTOs.Course;
using CourseTech.Shared;

namespace CourseTech.Core.Services
{
    public interface IEnrollmentService
    {
        Task<ServiceResult> EnrollAsync(Guid userId, Guid courseId);
        Task<ServiceResult> UnenrollAsync(Guid userId, Guid courseId);
        Task<ServiceResult<IEnumerable<CourseDTO>>> GetEnrolledCoursesByUserAsync(Guid userId);
        Task<ServiceResult<IEnumerable<AppUserDTO>>> GetEnrolledUsersByCourseAsync(Guid courseId);
    }
}