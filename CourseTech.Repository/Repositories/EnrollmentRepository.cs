using CourseTech.Core.Models;
using CourseTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseTech.Repository.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Enrollment?> GetEnrollmentAsync(Guid userId, Guid courseId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.AppUserId == userId && e.CourseId == courseId);
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByUserAsync(Guid userId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .Include(e => e.Course.Category)
                .Where(e => e.AppUserId == userId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(Guid courseId)
        {
            return await _context.Enrollments
                .Include(e => e.AppUser)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }
    }
}