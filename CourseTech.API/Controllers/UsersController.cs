using CourseTech.Core.DTOs.AppUser;
using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseTech.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : CustomBaseController
    {
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await userService.GetByIdAsync(id);
            return CreateActionResult(result);
        }

        [HttpGet("instructors")]
        public async Task<IActionResult> GetInstructorsAsync()
        {
            var result = await userService.GetInstructorsAsync();
            return CreateActionResult(result);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudentsAsync()
        {
            var result = await userService.GetStudentsAsync();
            return CreateActionResult(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var result = await userService.GetAllAsync();
            return CreateActionResult(result);
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Create(AppUserWithPasswordDTO createUserDto)
        {
            var result = await userService.CreateAsync(createUserDto);
            return CreateActionResult(result);
        }
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var result = await userService.ResetPasswordAsync(resetPasswordDTO.Email, resetPasswordDTO.NewPassword);

            return CreateActionResult(result);
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(AppUserDTO updateUserDto)
        {
            var result = await userService.UpdateAsync(updateUserDto);
            return CreateActionResult(result);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await userService.SoftDeleteAsync(id);
            return CreateActionResult(result);
        }
    }
}