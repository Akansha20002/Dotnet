namespace CourseTech.Core.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = default!;
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}