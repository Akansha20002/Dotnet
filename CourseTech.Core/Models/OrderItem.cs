namespace CourseTech.Core.Models
{
    public class OrderItem
    {
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } 
        public Guid CourseId { get; private set; }
        public Course Course { get; private set; } 
        public decimal Price { get; private set; }
        
        private OrderItem() { }
        
        public OrderItem(Guid orderId,Course course)
        {
            OrderId = orderId;
            CourseId = course.Id;
            Course = course;
            Price = course.Price;
        }
    }
}