namespace CourseTech.Core.DTOs.AppUser
{
    public record AppUserCreateDTO
    (
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string PhoneNumber
    );
}