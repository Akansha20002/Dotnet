using AutoMapper;
using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Models;
using CourseTech.Core.Models.Authentication;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Shared;
using Microsoft.AspNetCore.Identity;

namespace CourseTech.Service.Services
{
    public class AuthenticationService(IUnitOfWork unitOfWork, IIdentityTokenService tokenService, UserManager<AppUser> userManager, IMapper mapper) : IAuthenticationService
    {
        public async Task<ServiceResult<TokenDTO>> CreateTokenAsync(LoginDTO loginDto)
        {
            if (loginDto == null)
                return ServiceResult<TokenDTO>.Fail("Login information is required.");

            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return ServiceResult<TokenDTO>.Fail("Email or Password is wrong.");

            if (!await userManager.CheckPasswordAsync(user, loginDto.Password))
                return ServiceResult<TokenDTO>.Fail("Email or Password is wrong.");

            var token = tokenService.CreateToken(user);

            var userRefreshToken = await unitOfWork.AppUserRefreshToken.GetAsync(x => x.UserId == user.Id);
            if (userRefreshToken == null)
            {
                await unitOfWork.AppUserRefreshToken.AddRefreshTokenAsync(mapper.Map<AppUserRefreshToken>(new AppUserRefreshTokenDTO
                (
                    UserId: user.Id,
                    Token: token.RefreshToken,
                    ExpiresAt: token.RefreshTokenExpiresAt
                )));
            }
            else
            {
                userRefreshToken.Token = token.RefreshToken;
                userRefreshToken.ExpiresAt = token.RefreshTokenExpiresAt;
            }

            await unitOfWork.SaveChangesAsync();

            return ServiceResult<TokenDTO>.Success(token);
        }

        public async Task<ServiceResult<TokenDTO>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existingRefreshToken = await unitOfWork.AppUserRefreshToken.GetAsync(x => x.Token == refreshToken);
            if (existingRefreshToken == null)
                return ServiceResult<TokenDTO>.Fail("Refresh token not found.");

            if(!(existingRefreshToken.ExpiresAt > DateTime.UtcNow))
                return ServiceResult<TokenDTO>.Fail("Refresh token is expired.");

            var user = await userManager.FindByIdAsync(existingRefreshToken.UserId.ToString());
            if (user == null)
                return ServiceResult<TokenDTO>.Fail("User not found.");

            var token = tokenService.CreateToken(user);

            existingRefreshToken.Token = token.RefreshToken;
            existingRefreshToken.ExpiresAt = token.RefreshTokenExpiresAt;

            await unitOfWork.SaveChangesAsync();

            return ServiceResult<TokenDTO>.Success(token);
        }

        public async Task<ServiceResult> RevokeRefreshToken(string refreshToken)
        {
            var existingRefreshToken = await unitOfWork.AppUserRefreshToken.GetAsync(x => x.Token == refreshToken);
            if (existingRefreshToken == null)
                return ServiceResult.Fail("Refresh token not found.");

            await unitOfWork.AppUserRefreshToken.RemoveRefreshTokenAsync(existingRefreshToken.Token);
            await unitOfWork.SaveChangesAsync();
            return ServiceResult.Success(); 
        }
    }
}