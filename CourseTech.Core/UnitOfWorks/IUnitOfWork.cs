using CourseTech.Core.Repositories;
using CourseTech.Core.Services;

namespace CourseTech.Core.UnitOfWorks
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ICourseRepository Course { get; }
        IEnrollmentRepository Enrollment { get; }
        IBasketRepository Basket { get; }
        IAppUserRefreshTokenRepository AppUserRefreshToken { get; }
        IPaymentRepository Payment { get; }
        IOrderRepository Order { get; }
        Task SaveChangesAsync();
        void SaveChanges();
    }
}