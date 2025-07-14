using CourseTech.Core.Models;
using CourseTech.Core.Repositories;
using CourseTech.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseTech.Repository.Repositories
{
    public class BasketRepository : GenericRepository<Basket>, IBasketRepository
    {
        public BasketRepository(AppDbContext context) : base(context) { }

        public async Task<Basket?> GetBasketByUserIdAsync(Guid userId)
        {
            return await _entities
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.Course)
                .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Status == BasketStatus.Active);
        }

        public async Task<Basket?> GetBasketWithItemsAsync(Guid basketId)
        {
            return await _entities
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.Course)
                .FirstOrDefaultAsync(b => b.Id == basketId);
        }
    }
}
