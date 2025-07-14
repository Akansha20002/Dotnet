using CourseTech.Core.Models;

namespace CourseTech.Core.Repositories
{
    public interface IBasketRepository : IGenericRepository<Basket>
    {
        Task<Basket?> GetBasketByUserIdAsync(Guid userId);
        Task<Basket?> GetBasketWithItemsAsync(Guid basketId); // for admin panel, to see basket items of a user
    }
}