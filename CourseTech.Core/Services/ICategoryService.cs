using CourseTech.Core.DTOs.Category;
using CourseTech.Shared;

namespace CourseTech.Core.Services
{
    public interface ICategoryService
    {
        Task<ServiceResult<CategoryDTO>> GetByIdAsync(Guid categoryId);
        Task<ServiceResult<IEnumerable<CategoryDTO>>> GetAllAsync();
        Task<ServiceResult<IEnumerable<CategoryDTO>>> GetCategoryWithCoursesAsync(Guid id);
        Task<ServiceResult<IEnumerable<CategoryDTO>>> GetCategoriesWithCoursesAsync();

        Task<ServiceResult<CategoryDTO>> CreateAsync(CategoryCreateDTO categoryDto);
        Task<ServiceResult<CategoryDTO>> UpdateAsync(CategoryUpdateDTO categoryDto);
        Task<ServiceResult> SoftDeleteAsync(Guid categoryId);
       
    }
}