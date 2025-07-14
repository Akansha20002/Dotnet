using CourseTech.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseTech.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController(IEnrollmentService service) : CustomBaseController
    {
        [HttpPost]
        public async Task<IActionResult> Enroll(Guid userId, Guid courseId)
        {
            var result = await service.EnrollAsync(userId, courseId);
            return CreateActionResult(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Unenroll(Guid userId, Guid courseId)
        {
            var result = await service.UnenrollAsync(userId, courseId);
            return CreateActionResult(result);
        }

        [HttpGet("user/{userId:Guid}")]
        public async Task<IActionResult> GetEnrolledCoursesByUser(Guid userId)
        {
            var result = await service.GetEnrolledCoursesByUserAsync(userId);
            return CreateActionResult(result);
        }

        [HttpGet("course/{courseId:Guid}")]
        public async Task<IActionResult> GetEnrolledUsersByCourse(Guid courseId)
        {
            var result = await service.GetEnrolledUsersByCourseAsync(courseId);
            return CreateActionResult(result);
        }
    }
}