using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Models;
using CourseTech.Core.Models.Authentication;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Service.Services;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TestProject2.Services
{
    public class AuthenticationTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IIdentityTokenService> _tokenServiceMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly AuthenticationService _authService;

        public AuthenticationTest()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);

            _authService = new AuthenticationService(
                _unitOfWorkMock.Object,
                _tokenServiceMock.Object,
                _userManagerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldFail_IfLoginDtoIsNull()
        {
            var result = await _authService.CreateTokenAsync(null);
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldFail_IfUserNotFound()
        {
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((AppUser)null!);

            var login = new LoginDTO("user@test.com", "password");
            var result = await _authService.CreateTokenAsync(login);

            Assert.True(result.IsFailure);
            Assert.Contains("Email or Password is wrong", result.ErrorMessage![0]);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldFail_IfPasswordIsIncorrect()
        {
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser());

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var login = new LoginDTO("user@test.com", "wrongpassword");
            var result = await _authService.CreateTokenAsync(login);

            Assert.True(result.IsFailure);
            Assert.Contains("Email or Password is wrong", result.ErrorMessage![0]);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldReturnToken_IfLoginIsSuccessful()
        {
            var user = new AppUser { Id = Guid.NewGuid() };
            var token = new TokenDTO("accessToken", DateTime.UtcNow.AddMinutes(10), "refreshToken", DateTime.UtcNow.AddDays(1));

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
            _tokenServiceMock.Setup(x => x.CreateToken(user)).Returns(token);
            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync((AppUserRefreshToken)null!);
            _mapperMock.Setup(x => x.Map<AppUserRefreshToken>(It.IsAny<AppUserRefreshTokenDTO>()))
                .Returns(new AppUserRefreshToken());

            var login = new LoginDTO("user@test.com", "correctPassword");
            var result = await _authService.CreateTokenAsync(login);

            Assert.True(result.IsSuccess);
            Assert.Equal("accessToken", result.Data!.AccessToken);
        }

        [Fact]
        public async Task CreateTokenByRefreshToken_ShouldFail_IfTokenNotFound()
        {
            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync((AppUserRefreshToken)null!);

            var result = await _authService.CreateTokenByRefreshToken("invalidToken");

            Assert.True(result.IsFailure);
            Assert.Contains("Refresh token not found", result.ErrorMessage![0]);
        }

        [Fact]
        public async Task CreateTokenByRefreshToken_ShouldFail_IfTokenExpired()
        {
            var token = new AppUserRefreshToken { ExpiresAt = DateTime.UtcNow.AddMinutes(-5) };
            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync(token);

            var result = await _authService.CreateTokenByRefreshToken("expiredToken");

            Assert.True(result.IsFailure);
            Assert.Contains("Refresh token is expired", result.ErrorMessage![0]);
        }

        [Fact]
        public async Task CreateTokenByRefreshToken_ShouldSucceed_IfTokenIsValid()
        {
            var user = new AppUser { Id = Guid.NewGuid() };
            var refreshToken = new AppUserRefreshToken { UserId = user.Id, ExpiresAt = DateTime.UtcNow.AddMinutes(30) };
            var token = new TokenDTO("access", DateTime.UtcNow.AddMinutes(10), "refresh", DateTime.UtcNow.AddDays(1));

            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync(refreshToken);
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.CreateToken(user)).Returns(token);

            var result = await _authService.CreateTokenByRefreshToken("validToken");

            Assert.True(result.IsSuccess);
            Assert.Equal("access", result.Data!.AccessToken);
        }

        [Fact]
        public async Task RevokeRefreshToken_ShouldFail_IfTokenNotFound()
        {
            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync((AppUserRefreshToken)null!);

            var result = await _authService.RevokeRefreshToken("invalidToken");

            Assert.True(result.IsFail);
            Assert.Contains("Refresh token not found", result.ErrorMessage![0]);
        }

        [Fact]
        public async Task RevokeRefreshToken_ShouldSucceed_IfTokenExists()
        {
            var token = new AppUserRefreshToken { Token = "validToken" };

            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync(token);

            var result = await _authService.RevokeRefreshToken("validToken");

            Assert.True(result.IsSuccess);
        }
    }
}
