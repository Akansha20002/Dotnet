using CourseTech.Core.Repositories;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Repository.Repositories;

namespace CourseTech.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ICategoryRepository Category { get; private set; }
        public ICourseRepository Course { get; private set; }
        public IEnrollmentRepository Enrollment { get; private set; }
        public IBasketRepository Basket { get; private set; }
        public IAppUserRefreshTokenRepository AppUserRefreshToken { get; private set; }
        public IPaymentRepository Payment { get; private set; }
        public IOrderRepository Order { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(context);
            Course = new CourseRepository(context);
            Enrollment = new EnrollmentRepository(context);
            Basket = new BasketRepository(context);
            AppUserRefreshToken = new AppUserRefreshTokenRepository(context);
            Payment = new PaymentRepository(context);
            Order = new OrderRepository(context);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}