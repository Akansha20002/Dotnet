using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseTech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController(IAuthenticationService service) : CustomBaseController
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var result = await service.CreateTokenAsync(loginDto);

            return CreateActionResult(result);
        }

        [HttpPost("create-token-by-refresh-token")]
        public async Task<IActionResult> CreateTokenByRefreshToken(AppUserRefreshTokenDTO refreshTokenDTO)

        {
            var result = await service.CreateTokenByRefreshToken(refreshTokenDTO.Token);

            return CreateActionResult(result);
        }

        [Authorize]
        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefreshToken(AppUserRefreshTokenDTO refreshTokenDTO)
        {
            var result = await service.RevokeRefreshToken(refreshTokenDTO.Token);

            return CreateActionResult(result);
        }
    }
}