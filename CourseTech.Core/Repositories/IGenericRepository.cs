using CourseTech.Core.Models;
using System.Linq.Expressions;

namespace CourseTech.Core.Repositories
{
    public interface IGenericRepository<TEntity>
        where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        Task InsertAsync(TEntity entity);
        TEntity Update(TEntity entity);
        void SoftDelete(TEntity entity);
    }
}