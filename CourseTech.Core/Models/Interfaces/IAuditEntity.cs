namespace CourseTech.Core.Models.Interfaces
{
    public interface IAuditEntity : IBaseEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}