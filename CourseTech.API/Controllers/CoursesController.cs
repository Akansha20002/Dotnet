using CourseTech.Core.DTOs.Course;
using CourseTech.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseTech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController(ICourseService service) : CustomBaseController
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

        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedCourses()
        {
            var result = await service.GetPublishedCoursesAsync();
            return CreateActionResult(result);
        }

        [HttpGet("by-category/{categoryId:Guid}")]
        public async Task<IActionResult> GetCoursesByCategory(Guid categoryId)
        {
            var result = await service.GetCoursesByCategoryAsync(categoryId);
            return CreateActionResult(result);
        }

        [HttpGet("by-instructor/{instructorId:Guid}")]
        public async Task<IActionResult> GetCoursesByInstructor(Guid instructorId)
        {
            var result = await service.GetCoursesByInstructorAsync(instructorId);
            return CreateActionResult(result);
        }

        [HttpGet("details/{id:Guid}")]
        public async Task<IActionResult> GetCourseWithDetails(Guid id)
        {
            var result = await service.GetCourseWithDetailsAsync(id);
            return CreateActionResult(result);
        }

        [HttpGet("summaries")]
        public async Task<IActionResult> GetAllCoursesSummariesForCard()
        {
            var result = await service.GetAllCoursesSummariesForCardAsync();
            return CreateActionResult(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CourseCreateDTO courseDto)
        {
            var result = await service.CreateAsync(courseDto);
            return CreateActionResult(result);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CourseUpdateDTO courseDto)
        {
            var result = await service.UpdateAsync(courseDto);
            return CreateActionResult(result);
        }

        [Authorize]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var result = await service.SoftDeleteAsync(id);
            return CreateActionResult(result);
        }

        [Authorize]
        [HttpPatch("{id:Guid}/publish")]
        public async Task<IActionResult> PublishCourse(Guid id)
        {
            var result = await service.PublishCourseAsync(id);
            return CreateActionResult(result);
        }

        [Authorize]
        [HttpPatch("{id:Guid}/unpublish")]
        public async Task<IActionResult> UnpublishCourse(Guid id)
        {
            var result = await service.UnpublishCourseAsync(id);
            return CreateActionResult(result);
        }
    }
}