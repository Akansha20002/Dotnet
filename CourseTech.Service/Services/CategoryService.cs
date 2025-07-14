using AutoMapper;
using CourseTech.Core.DTOs.Category;
using CourseTech.Core.Models;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Shared;

namespace CourseTech.Service.Services
{
    public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
    {
        public async Task<ServiceResult<CategoryDTO>> GetByIdAsync(Guid categoryId)
        {
            var entity = await unitOfWork.Category.GetByIdAsync(categoryId);
            if (entity == null)
                return ServiceResult<CategoryDTO>.Fail($"Category ({categoryId}) not found.");

            var categoryDto = mapper.Map<CategoryDTO>(entity);

            return ServiceResult<CategoryDTO>.Success(categoryDto);
        }

        public async Task<ServiceResult<IEnumerable<CategoryDTO>>> GetAllAsync()
        {
            var entities = await unitOfWork.Category.GetAllAsync();
            var categoryDtos = mapper.Map<IEnumerable<CategoryDTO>>(entities);

            return ServiceResult<IEnumerable<CategoryDTO>>.Success(categoryDtos);
        }

        public async Task<ServiceResult<IEnumerable<CategoryDTO>>> GetCategoriesWithCoursesAsync()
        {
            var categoriesWithCourses = await unitOfWork.Category.GetCategoriesWithCoursesAsync();
            var categoryDtos = mapper.Map<IEnumerable<CategoryDTO>>(categoriesWithCourses);

            return ServiceResult<IEnumerable<CategoryDTO>>.Success(categoryDtos);
        }

        public async Task<ServiceResult<IEnumerable<CategoryDTO>>> GetCategoryWithCoursesAsync(Guid id)
        {
            var categoryWithCourses = await unitOfWork.Category.GetCategoryWithCoursesAsync(id);
            var categoryDtos = mapper.Map<IEnumerable<CategoryDTO>>(categoryWithCourses);

            return ServiceResult<IEnumerable<CategoryDTO>>.Success(categoryDtos);
        }

        public async Task<ServiceResult<CategoryDTO>> CreateAsync(CategoryCreateDTO categoryDto)
        {
            var entity = mapper.Map<Category>(categoryDto);
            await unitOfWork.Category.InsertAsync(entity);
            await unitOfWork.SaveChangesAsync();
            var newCategoryDto = mapper.Map<CategoryDTO>(entity);

            return ServiceResult<CategoryDTO>.Success(newCategoryDto);
        }
        public async Task<ServiceResult<CategoryDTO>> UpdateAsync(CategoryUpdateDTO categoryDto)
        {
            var existingEntity = await unitOfWork.Category.GetByIdAsync(categoryDto.Id);
            if (existingEntity == null)
                return ServiceResult<CategoryDTO>.Fail($"Category ({categoryDto.Id}) not found.");

            mapper.Map(categoryDto, existingEntity);

            unitOfWork.Category.Update(existingEntity);
            await unitOfWork.SaveChangesAsync();

            var updatedCategoryDto = mapper.Map<CategoryDTO>(existingEntity);
            return ServiceResult<CategoryDTO>.Success(updatedCategoryDto);
        }
        public async Task<ServiceResult> SoftDeleteAsync(Guid categoryId)
        {
            var entity = await unitOfWork.Category.GetByIdAsync(categoryId);
            if (entity == null)
                return ServiceResult.Fail($"Category ({categoryId}) not found.");

            entity.MarkAsDeleted();
            unitOfWork.Category.Update(entity);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }
    }
}