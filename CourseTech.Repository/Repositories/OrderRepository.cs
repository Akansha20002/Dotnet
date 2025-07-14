using CourseTech.Core.Models;
using CourseTech.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CourseTech.Repository.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
        
        public async Task<Order?> GetOrderByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .Include(o => o.AppUser)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Course)
                .Include(o => o.AppUser)
                .Where(o => o.UserId == userId && o.Status == Shared.Enums.OrderStatus.Completed)
                .ToListAsync(cancellationToken);
        }
    }
}