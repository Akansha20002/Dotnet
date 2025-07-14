namespace CourseTech.Core.Models
{
    public class BasketItem
    {
        public Guid BasketId { get; set; }
        public Basket Basket { get; set; }

        public Guid CourseId { get; private set; }
        public Course Course { get; private set; }

        public decimal Price => Course.Price;
        
        private BasketItem() { }
        public BasketItem(Guid basketId,Course course)
        {
            BasketId = basketId;
            CourseId = course.Id;
            Course = course;
        }   
    }
}