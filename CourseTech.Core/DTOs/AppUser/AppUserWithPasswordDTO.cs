namespace CourseTech.Core.DTOs.AppUser
{
    public record AppUserWithPasswordDTO(Guid Id, string FirstName, string LastName, string Email,string Password, string PhoneNumber);
}