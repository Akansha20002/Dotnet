using CourseTech.Core.DTOs.Authentication;
using CourseTech.Shared;

namespace CourseTech.Core.Services
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<TokenDTO>> CreateTokenAsync(LoginDTO loginDto);
        Task<ServiceResult<TokenDTO>> CreateTokenByRefreshToken(string refreshToken);
        Task<ServiceResult> RevokeRefreshToken(string refreshToken);
    }
}