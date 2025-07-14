


using CourseTech.Core.DTOs.Category;
using CourseTech.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseTech.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService service) : CustomBaseController
    {
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await service.GetByIdAsync(id);
            return CreateActionResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await service.GetAllAsync();
            return CreateActionResult(result);
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCategoriesWithCourses()
        {
            var result = await service.GetCategoriesWithCoursesAsync();
            return CreateActionResult(result);
        }

        [HttpGet("{id}/courses")]
        public async Task<IActionResult> GetCategoryWithCourses(Guid id)
        {
            var result = await service.GetCategoryWithCoursesAsync(id);
            return CreateActionResult(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO categoryDto)
        {
            var result = await service.CreateAsync(categoryDto);
            return CreateActionResult(result);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateDTO categoryDto)
        {
            var result = await service.UpdateAsync(categoryDto);
            return CreateActionResult(result);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await service.SoftDeleteAsync(id);
            return CreateActionResult(result);
        }
    }
}