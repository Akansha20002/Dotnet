using CourseTech.Shared.Enums;

namespace CourseTech.Core.Models
{
    public class Course : BaseEntity
    {
        // Properties
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public string VideoUrl { get; set; } = default!;
        public CourseLevel Level { get; set; }
        public CourseLanguage Language { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsPublished { get; private set; }
        public DateTime? PublishedAt { get; private set; }

        // Relationships
        public Guid InstructorId { get; set; } = default!;
        public AppUser? Instructor { get; set; }

        public Guid CategoryId { get; set; } = default!;
        public Category? Category { get; set; }

        private Course() { }

        public Course(string title, string description, string imageUrl, string videoUrl, CourseLevel level, CourseLanguage language, decimal price, TimeSpan duration, Guid instructorId, Guid categoryId)
        {
            Title = title;
            Description = description;
            ImageUrl = imageUrl;
            VideoUrl = videoUrl;
            Level = level;
            Language = language;
            Price = price;
            Duration = duration;
            InstructorId = instructorId;
            CategoryId = categoryId;
        }

        public void Publish()
        {
            if (!IsPublished)
            {
                IsPublished = true;
                PublishedAt = DateTime.UtcNow;
                Update();
            }
        }

        public void Unpublish()
        {
            if (IsPublished)
            {
                IsPublished = false;
                PublishedAt = null;
                Update();
            }
        }
    }
}