namespace CourseTech.Core.Models
{
    public class Enrollment : BaseEntity
    {
        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}