using AutoMapper;
using CourseTech.Core.DTOs.AppUser;
using CourseTech.Core.Models;
using CourseTech.Core.Services;
using CourseTech.Shared;
using CourseTech.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseTech.Service.Services
{
    public class UserService(UserManager<AppUser> userManager, IMapper mapper) : IUserService
    {
        public async Task<ServiceResult<AppUserDTO>> GetByIdAsync(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return ServiceResult<AppUserDTO>.Fail($"User ({id}) not found.");

            return ServiceResult<AppUserDTO>.Success(mapper.Map<AppUserDTO>(user));
        }

        public async Task<ServiceResult<IEnumerable<AppUserWithNamesDTO>>> GetInstructorsAsync()
        {
            var users = await userManager.GetUsersInRoleAsync(Roles.Instructor.ToString());
            if (!users.Any())
                return ServiceResult<IEnumerable<AppUserWithNamesDTO>>.Fail("No instructor found.");

            var instructorDtos = mapper.Map<IEnumerable<AppUserWithNamesDTO>>(users);
            return ServiceResult<IEnumerable<AppUserWithNamesDTO>>.Success(instructorDtos);
        }

        public async Task<ServiceResult<IEnumerable<AppUserWithNamesDTO>>> GetStudentsAsync()
        {
            var users = await userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            if (!users.Any())
                return ServiceResult<IEnumerable<AppUserWithNamesDTO>>.Fail("No student found.");

            var studentDtos = mapper.Map<IEnumerable<AppUserWithNamesDTO>>(users);
            return ServiceResult<IEnumerable<AppUserWithNamesDTO>>.Success(studentDtos);
        }

        public async Task<ServiceResult<IEnumerable<AppUserDTO>>> GetAllAsync()
        {
            var users = await userManager.Users.ToListAsync();
            var userDtos = mapper.Map<IEnumerable<AppUserDTO>>(users);
            return ServiceResult<IEnumerable<AppUserDTO>>.Success(userDtos);
        }

        public async Task<ServiceResult<AppUserDTO>> CreateAsync(AppUserWithPasswordDTO createUserDto)
        {
            var user = new AppUser
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                PhoneNumber = createUserDto.PhoneNumber,
                UserName = createUserDto.Email
            };

            var createResult = await userManager.CreateAsync(user, createUserDto.Password);
            if (!createResult.Succeeded)
                return ServiceResult<AppUserDTO>.Fail(createResult.Errors.Select(e => e.Description).ToList());

            var roleResult = await userManager.AddToRoleAsync(user, Roles.Student.ToString());
            if (!roleResult.Succeeded)
                return ServiceResult<AppUserDTO>.Fail(roleResult.Errors.Select(e => e.Description).ToList());

            var userDto = mapper.Map<AppUserDTO>(user);
            return ServiceResult<AppUserDTO>.Success(userDto);
        }

        public async Task<ServiceResult> AssignRoleAsync(Guid id, string roleName)
        {
            var existingUser = await userManager.FindByIdAsync(id.ToString());
            if (existingUser == null)
                return ServiceResult.Fail($"User ({id}) not found.");

            var addRole = await userManager.AddToRoleAsync(existingUser, roleName);
            if (!addRole.Succeeded)
                return ServiceResult.Fail(addRole.Errors.Select(e => e.Description).ToList());

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ResetPasswordAsync(string email, string newPassord)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return ServiceResult.Fail($"User ({email}) not found.");
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, token, newPassord);
            if (!result.Succeeded)
                return ServiceResult.Fail(result.Errors.Select(e => e.Description).ToList());

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UpdateAsync(AppUserDTO updateUserDto)
        {
            var user = await userManager.FindByIdAsync(updateUserDto.Id.ToString());
            if (user == null)
                return ServiceResult.Fail($"User ({updateUserDto.Id}) not found.");

            mapper.Map(updateUserDto, user);
            user.Update();
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return ServiceResult.Fail(result.Errors.Select(e => e.Description).ToList());

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> SoftDeleteAsync(Guid id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return ServiceResult.Fail($"User ({id}) not found.");

            user.MarkAsDeleted();
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return ServiceResult.Fail(result.Errors.Select(e => e.Description).ToList());

            return ServiceResult.Success();
        }
    }
}