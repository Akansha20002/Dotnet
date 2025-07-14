using CourseTech.Core.DTOs.Basket;
using CourseTech.Shared;

namespace CourseTech.Core.Services
{
    public interface IBasketService
    {
        Task<ServiceResult<BasketDTO>> GetActiveBasketAsync(Guid userId);
        Task<ServiceResult> AddCourseToBasketAsync(Guid userId, Guid courseId);
        Task<ServiceResult> RemoveCourseFromBasketAsync(Guid userId, Guid courseId);
        Task<ServiceResult> ClearBasketAsync(Guid userId);
        Task<ServiceResult> CompleteBasketAsync(Guid userId);
        Task<ServiceResult<BasketDTO>> GetBasketWithItemsAsync(Guid basketId);
    }
}