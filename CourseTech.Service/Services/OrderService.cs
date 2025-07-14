using AutoMapper;
using CourseTech.Core.DTOs.Order;
using CourseTech.Core.Models;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Shared;
using Microsoft.Extensions.Logging;

namespace CourseTech.Service.Services
{
    public class OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger) : IOrderService
    {
        // eğer basket controllerda basket complete succces dönerse order service çağrılır ve order oluşturulur
        public async Task<ServiceResult<OrderDTO>> CreateOrderFromBasketAsync(Guid basketId)
        {
            var basket = await unitOfWork.Basket.GetBasketWithItemsAsync(basketId);
            if (basket == null)
                return ServiceResult<OrderDTO>.Fail("Basket not found.");

            var order = new Order(basket.UserId, basket.AppUser);

            foreach (var basketItem in basket.BasketItems)
            {
                order.AddOrderItem(basketItem.Course);
            }

            await unitOfWork.Order.InsertAsync(order);
            unitOfWork.Basket.SoftDelete(basket);
            order.MarkAsPending();
            await unitOfWork.SaveChangesAsync();

            var orderDTO = mapper.Map<OrderDTO>(order);

            logger.LogInformation("Order {OrderId} created from basket {BasketId}", order.Id, basket.Id);
            return ServiceResult<OrderDTO>.Success(orderDTO);
        }

        public async Task<ServiceResult<OrderDTO>> GetOrderByIdAsync(Guid orderId)
        {
            var order = await unitOfWork.Order.GetOrderByIdWithIncludesAsync(orderId);
            if (order == null)
                return ServiceResult<OrderDTO>.Fail("Order not found.");

            var orderDTO = mapper.Map<OrderDTO>(order);
            return ServiceResult<OrderDTO>.Success(orderDTO);
        }

        public async Task<ServiceResult<List<OrderDTO>>> GetOrdersByUserIdAsync(Guid userId)
        {
            var orders = await unitOfWork.Order.GetOrdersByUserIdAsync(userId);
            var orderDTOs = mapper.Map<List<OrderDTO>>(orders);
            return ServiceResult<List<OrderDTO>>.Success(orderDTOs);
        }
    }
}