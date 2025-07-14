using Microsoft.AspNetCore.Identity;

namespace CourseTech.Core.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; private set; } = false;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Basket> Baskets { get; set; } = new List<Basket>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            Update();
        }

        public void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}