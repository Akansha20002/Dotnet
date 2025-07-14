using CourseTech.Core.Models;

namespace CourseTech.Core.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithCoursesAsync();
        Task<IEnumerable<Category>> GetCategoryWithCoursesAsync(Guid id);
    }
}