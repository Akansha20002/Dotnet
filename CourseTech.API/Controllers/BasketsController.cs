using CourseTech.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseTech.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController(IBasketService service) : CustomBaseController
    {
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetBasket(Guid userId)
        {
            var result = await service.GetActiveBasketAsync(userId);
            return CreateActionResult(result);
        }

        [HttpPost("users/{userId}/courses/{courseId}")]
        public async Task<IActionResult> AddCourseToBasket(Guid userId, Guid courseId)
        {
            var result = await service.AddCourseToBasketAsync(userId, courseId);
            return CreateActionResult(result);
        }

        [HttpDelete("users/{userId}/courses/{courseId}")]
        public async Task<IActionResult> RemoveCourseFromBasket(Guid userId, Guid courseId)
        {
            var result = await service.RemoveCourseFromBasketAsync(userId, courseId);
            return CreateActionResult(result);
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> ClearBasket(Guid userId)
        {
            var result = await service.ClearBasketAsync(userId);
            return CreateActionResult(result);
        }

        [HttpPost("users/{userId}/complete")]
        public async Task<IActionResult> CompleteBasket(Guid userId)
        {
            var result = await service.CompleteBasketAsync(userId);
            return CreateActionResult(result);
        }

        [HttpGet("{basketId:guid}/admin")]
        public async Task<IActionResult> GetBasketWithItems(Guid basketId)
        {
            var result = await service.GetBasketWithItemsAsync(basketId);
            return CreateActionResult(result);
        }
    }
}