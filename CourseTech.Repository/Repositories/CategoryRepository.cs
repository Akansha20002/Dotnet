using CourseTech.Core.Models;
using CourseTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseTech.Repository.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Category>> GetCategoriesWithCoursesAsync()
        {
            return await _entities.Include(c => c.Courses)
                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoryWithCoursesAsync(Guid id)
        {
            return await _entities.Include(c => c.Courses)
                .Where(c => c.Id == id && !c.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}