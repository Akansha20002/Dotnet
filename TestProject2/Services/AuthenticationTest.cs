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

namespace TestProject2.Xunit.Services
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

        private LoginDTO CreateLoginDTO(string email = "user@test.com", string password = "password")
            => new(email, password);

        private TokenDTO CreateTokenDTO()
            => new("accessToken", DateTime.UtcNow.AddMinutes(10), "refreshToken", DateTime.UtcNow.AddDays(1));

        [Fact, Trait("Category", "CreateToken")]
        public async Task CreateTokenAsync_WhenLoginDtoIsNull_ShouldReturnFailure()
        {
            var result = await _authService.CreateTokenAsync(null);
            Assert.True(result.IsFailure);
            Assert.Contains("Login information is required", result.ErrorMessage?[0] ?? "", StringComparison.OrdinalIgnoreCase);
        }

        [Fact, Trait("Category", "CreateToken")]
        public async Task CreateTokenAsync_WhenUserNotFound_ShouldReturnFailure()
        {
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((AppUser)null!);

            var result = await _authService.CreateTokenAsync(CreateLoginDTO());

            Assert.True(result.IsFailure);
            Assert.Contains("Email or Password is wrong", result.ErrorMessage![0]);
        }

        [Fact, Trait("Category", "CreateToken")]
        public async Task CreateTokenAsync_WhenPasswordIncorrect_ShouldReturnFailure()
        {
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new AppUser());

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _authService.CreateTokenAsync(CreateLoginDTO("user@test.com", "wrongPassword"));

            Assert.True(result.IsFailure);
            Assert.Contains("Email or Password is wrong", result.ErrorMessage![0]);
        }

        [Fact, Trait("Category", "CreateToken")]
        public async Task CreateTokenAsync_WhenCredentialsValid_ShouldReturnToken()
        {
            var user = new AppUser { Id = Guid.NewGuid() };
            var token = CreateTokenDTO();

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
            _tokenServiceMock.Setup(x => x.CreateToken(user)).Returns(token);
            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync((AppUserRefreshToken)null!);
            _mapperMock.Setup(x => x.Map<AppUserRefreshToken>(It.IsAny<AppUserRefreshTokenDTO>()))
                .Returns(new AppUserRefreshToken());

            var result = await _authService.CreateTokenAsync(CreateLoginDTO("user@test.com", "correctPassword"));

            Assert.True(result.IsSuccess);
            Assert.Equal(token.AccessToken, result.Data!.AccessToken);

            _userManagerMock.Verify(x => x.FindByEmailAsync("user@test.com"), Times.Once);
            _tokenServiceMock.Verify(x => x.CreateToken(user), Times.Once);
        }

        [Fact, Trait("Category", "RefreshToken")]
        public async Task CreateTokenByRefreshToken_WhenTokenNotFound_ShouldReturnFailure()
        {
            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync((AppUserRefreshToken)null!);

            var result = await _authService.CreateTokenByRefreshToken("invalidToken");

            Assert.True(result.IsFailure);
            Assert.Contains("Refresh token not found", result.ErrorMessage![0]);
        }

        [Fact, Trait("Category", "RefreshToken")]
        public async Task CreateTokenByRefreshToken_WhenTokenExpired_ShouldReturnFailure()
        {
            var expiredToken = new AppUserRefreshToken { ExpiresAt = DateTime.UtcNow.AddMinutes(-5) };

            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync(expiredToken);

            var result = await _authService.CreateTokenByRefreshToken("expiredToken");

            Assert.True(result.IsFailure);
            Assert.Contains("Refresh token is expired", result.ErrorMessage![0]);
        }

        [Fact, Trait("Category", "RefreshToken")]
        public async Task CreateTokenByRefreshToken_WhenTokenValid_ShouldReturnNewToken()
        {
            var user = new AppUser { Id = Guid.NewGuid() };
            var validToken = new AppUserRefreshToken { UserId = user.Id, ExpiresAt = DateTime.UtcNow.AddMinutes(30) };
            var expectedToken = CreateTokenDTO();

            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync(validToken);
            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.CreateToken(user)).Returns(expectedToken);

            var result = await _authService.CreateTokenByRefreshToken("validToken");

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedToken.AccessToken, result.Data!.AccessToken);
        }

        [Fact, Trait("Category", "RevokeToken")]
        public async Task RevokeRefreshToken_WhenTokenNotFound_ShouldReturnFailure()
        {
            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync((AppUserRefreshToken)null!);

            var result = await _authService.RevokeRefreshToken("invalidToken");

            Assert.True(result.IsFail);
            Assert.Contains("Refresh token not found", result.ErrorMessage![0]);
        }

        [Fact, Trait("Category", "RevokeToken")]
        public async Task RevokeRefreshToken_WhenTokenExists_ShouldSucceed()
        {
            var token = new AppUserRefreshToken { Token = "validToken" };

            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync(token);

            var result = await _authService.RevokeRefreshToken("validToken");

            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()), Times.Once);
        }
    }
}
