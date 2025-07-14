using CourseTech.Shared.Enums;

namespace CourseTech.Core.Models
{
    public class Basket : BaseEntity
    {
        public Guid UserId { get; set; } = default!;
        public AppUser AppUser { get; set; } = null!;

        public BasketStatus Status { get; private set; } = BasketStatus.Active;

        private readonly List<BasketItem> _basketItems = new();
        public IReadOnlyCollection<BasketItem> BasketItems => _basketItems.AsReadOnly();

        private decimal _totalPrice;
        public decimal TotalPrice
        {
            get => _totalPrice;
            private set => _totalPrice = value;
        }

        private Basket() {}

        public Basket(Guid userId)
        {
            UserId = userId;
            Status = BasketStatus.Active;
        }

        public void CalculateTotalPrice()
        {
            _totalPrice = BasketItems.Sum(x => x.Price);
        }

        public void AddCourse(Course course)
        {
            if (Status != BasketStatus.Active)
                throw new InvalidOperationException("Only active baskets can be modified.");

            if (BasketItems.Any(item => item.CourseId == course.Id))
                throw new InvalidOperationException("Course already exists in the basket.");

            _basketItems.Add(new BasketItem(this.Id, course));
            CalculateTotalPrice();
        }
        public void RemoveCourse(Guid courseId)
        {
            if (Status != BasketStatus.Active)
                throw new InvalidOperationException("Only active baskets can be modified.");

            var item = _basketItems.FirstOrDefault(i => i.CourseId == courseId);
            if (item == null)
                throw new InvalidOperationException("Course not found in the basket.");

            _basketItems.Remove(item);
            CalculateTotalPrice();
        }

        public void ClearBasket()
        {
            if (Status != BasketStatus.Active)
                throw new InvalidOperationException("Only active baskets can be cleared.");

            _basketItems.Clear();
            CalculateTotalPrice();
        }

        public void CompleteBasket()
        {
            if (!BasketItems.Any())
                throw new InvalidOperationException("Cannot complete an empty basket.");

            Status = BasketStatus.Passive;
            CalculateTotalPrice();
        }
    }
}