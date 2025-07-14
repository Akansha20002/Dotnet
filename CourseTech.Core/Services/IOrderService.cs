using CourseTech.Core.DTOs.Order;
using CourseTech.Shared;

namespace CourseTech.Core.Services
{
    public interface IOrderService
    {
        Task<ServiceResult<OrderDTO>> CreateOrderFromBasketAsync(Guid basketId);
        Task<ServiceResult<OrderDTO>> GetOrderByIdAsync(Guid orderId);
        Task<ServiceResult<List<OrderDTO>>> GetOrdersByUserIdAsync(Guid userId);
    }
}