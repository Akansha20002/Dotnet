using CourseTech.Core.Models;

namespace CourseTech.Core.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Order>> GetOrdersByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}