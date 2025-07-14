using CourseTech.Core.DTOs.AppUser;
using CourseTech.Shared;

namespace CourseTech.Core.Services
{
    public interface IUserService
    {
        Task<ServiceResult<AppUserDTO>> GetByIdAsync(Guid id);
        Task<ServiceResult<IEnumerable<AppUserWithNamesDTO>>> GetInstructorsAsync();
        Task<ServiceResult<IEnumerable<AppUserWithNamesDTO>>> GetStudentsAsync();
        Task<ServiceResult<IEnumerable<AppUserDTO>>> GetAllAsync(); // for Admin

        Task<ServiceResult<AppUserDTO>> CreateAsync(AppUserWithPasswordDTO createUserDto);
        Task<ServiceResult> UpdateAsync(AppUserDTO updateUserDto);
        Task<ServiceResult> SoftDeleteAsync(Guid id);

        Task<ServiceResult> AssignRoleAsync(Guid id, string roleName);
        Task<ServiceResult> ResetPasswordAsync(string email, string newPassword);
    }
}