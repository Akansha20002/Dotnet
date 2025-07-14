using CourseTech.Shared.Enums;

namespace CourseTech.Core.Models
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; private set; }
        public AppUser AppUser { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        
        private decimal _totalPrice;
        public decimal TotalPrice => _totalPrice;

        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public Payment? Payment { get; private set; }

        private Order() {}
        public Order(Guid userId, AppUser user)
        {
            UserId = userId;
            AppUser = user;
            Status = OrderStatus.Pending;
        }
        public void AddOrderItem(Course course)
        {
            if (_orderItems.Any(x => x.CourseId == course.Id))
                throw new InvalidOperationException("Course is already in the order.");

            _orderItems.Add(new OrderItem(this.Id,course));
            RecalculateTotalPrice();
        }
        public void MarkAsCompleted()
        {
            Status = OrderStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsPending()
        {
            Status = OrderStatus.Pending;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsCanceled()
        {
            Status = OrderStatus.Canceled;
            UpdatedAt = DateTime.UtcNow;
        }

        private void RecalculateTotalPrice()
        {
            _totalPrice = _orderItems.Sum(x => x.Price);
        }
    }
}
