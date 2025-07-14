using CourseTech.Core.Models.Interfaces;

namespace CourseTech.Core.Models
{
    public abstract class BaseEntity : IAuditEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; private set; } = false;

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