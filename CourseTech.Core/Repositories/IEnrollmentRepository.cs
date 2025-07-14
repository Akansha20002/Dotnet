using CourseTech.Core.Models;

namespace CourseTech.Core.Repositories
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetEnrollmentsByUserAsync(Guid userId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(Guid courseId);
        Task<Enrollment?> GetEnrollmentAsync(Guid userId, Guid courseId);
    }
}