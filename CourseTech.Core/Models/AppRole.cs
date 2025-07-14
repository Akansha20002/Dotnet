using CourseTech.Core.Models.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CourseTech.Core.Models
{
    public class AppRole : IdentityRole<Guid>, IAuditEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}