using AutoMapper;
using CourseTech.Core.DTOs.Basket;
using CourseTech.Core.Models;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Shared;

namespace CourseTech.Service.Services
{
    public class BasketService(IUnitOfWork unitOfWork, IMapper mapper) : IBasketService
    {
        public async Task<ServiceResult<BasketDTO>> GetActiveBasketAsync(Guid userId)
        {
            var basket = await unitOfWork.Basket.GetBasketByUserIdAsync(userId);
            if (basket == null)
            {
                basket = new Basket(userId);
                await unitOfWork.Basket.InsertAsync(basket);
                await unitOfWork.SaveChangesAsync();
            }

            var basketDto = mapper.Map<BasketDTO>(basket);
            return ServiceResult<BasketDTO>.Success(basketDto);
        }

        public async Task<ServiceResult> AddCourseToBasketAsync(Guid userId, Guid courseId)
        {
            var course = await unitOfWork.Course.GetByIdAsync(courseId);
            if (course == null)
                return ServiceResult.Fail("Course not found");

            var basket = await unitOfWork.Basket.GetBasketByUserIdAsync(userId);
            if (basket == null)
            {
                basket = new Basket(userId);
                await unitOfWork.Basket.InsertAsync(basket);
                await unitOfWork.SaveChangesAsync();
            }
            basket!.AddCourse(course);
            basket!.CalculateTotalPrice();

            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ClearBasketAsync(Guid userId)
        {
            var basket = await unitOfWork.Basket.GetBasketByUserIdAsync(userId);
            basket!.ClearBasket();
            basket.CalculateTotalPrice();

            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> CompleteBasketAsync(Guid userId)
        {
            var basket = await unitOfWork.Basket.GetBasketByUserIdAsync(userId);
            basket!.CompleteBasket();
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RemoveCourseFromBasketAsync(Guid userId, Guid courseId)
        {
            var basket = await unitOfWork.Basket.GetBasketByUserIdAsync(userId);
            basket!.RemoveCourse(courseId);
            basket.CalculateTotalPrice();

            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success();
        }

        public async Task<ServiceResult<BasketDTO>> GetBasketWithItemsAsync(Guid basketId)
        {
            var basket = await unitOfWork.Basket.GetBasketWithItemsAsync(basketId);
            if (basket == null)
                return ServiceResult<BasketDTO>.Fail("Basket not found.");

            var basketDto = mapper.Map<BasketDTO>(basket);
            return ServiceResult<BasketDTO>.Success(basketDto);
        }
    }
}