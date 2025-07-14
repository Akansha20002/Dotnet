using CourseTech.Core.Models;
using CourseTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseTech.Repository.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Course>> GetAllCoursesWithDetailsAsync()
        {
            return await _entities
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        {
            return await _entities
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Where(c => c.IsPublished && !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByInstructorAsync(Guid instructorId)
        {
            return await _entities
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(Guid categoryId)
        {
            return await _entities
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Where(c => c.CategoryId == categoryId && !c.IsDeleted && c.IsPublished) 
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course?> GetCourseWithDetailsAsync(Guid courseId)
        {
            return await _entities
                .Include(c => c.Category)
                .Include(c => c.Instructor)
                .Where(c => c.Id == courseId && !c.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}