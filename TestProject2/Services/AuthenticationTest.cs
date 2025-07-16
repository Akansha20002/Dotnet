using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CourseTech.Core.Models;
using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Models.Authentication;
using CourseTech.Core.Services;
using CourseTech.Core.UnitOfWorks;
using CourseTech.Service.Services;

namespace TestProject2.Xunit.Services
{
    public class AuthenticationTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IIdentityTokenService> _tokenServiceMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly AuthenticationService _authService;

        private const string LoginError = "Email or Password is wrong";
        private const string LoginNullError = "Login information is required";
        private const string RefreshTokenNotFound = "Refresh token not found";
        private const string RefreshTokenExpired = "Refresh token is expired";

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

        // ---------- Helper Methods ----------

        private static LoginDTO CreateLoginDTO(string email = "user@test.com", string password = "password") =>
            new(email, password);

        private static TokenDTO CreateTokenDTO() =>
            new("access-token", DateTime.UtcNow.AddMinutes(15), "refresh-token", DateTime.UtcNow.AddDays(1));

        private static AppUser CreateUser() =>
            new() { Id = Guid.NewGuid(), Email = "user@test.com", UserName = "user" };

        // ---------- CreateToken Tests ----------

        [Fact]
        public async Task CreateTokenAsync_WhenLoginDtoIsNull_ShouldReturnFailure()
        {
            // Act
            var result = await _authService.CreateTokenAsync(null);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(LoginNullError, result.ErrorMessage![0]);
        }

        [Fact]
        public async Task CreateTokenAsync_WhenUserNotFound_ShouldReturnFailure()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((AppUser)null!);

            // Act
            var result = await _authService.CreateTokenAsync(CreateLoginDTO());

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(LoginError, result.ErrorMessage![0]);
        }

        [Fact]
        public async Task CreateTokenAsync_WhenPasswordIncorrect_ShouldReturnFailure()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(CreateUser());

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await _authService.CreateTokenAsync(CreateLoginDTO("user@test.com", "wrongPassword"));

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(LoginError, result.ErrorMessage![0]);
        }

        [Fact]
        public async Task CreateTokenAsync_WhenCredentialsValid_ShouldReturnToken()
        {
            // Arrange
            var user = CreateUser();
            var token = CreateTokenDTO();

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(true);
            _tokenServiceMock.Setup(x => x.CreateToken(user)).Returns(token);

            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync((AppUserRefreshToken)null!);

            _mapperMock.Setup(x => x.Map<AppUserRefreshToken>(It.IsAny<AppUserRefreshTokenDTO>()))
                .Returns(new AppUserRefreshToken());

            // Act
            var result = await _authService.CreateTokenAsync(CreateLoginDTO(user.Email, "correctPassword"));

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(token.AccessToken, result.Data!.AccessToken);
        }

        // ---------- RefreshToken Tests ----------

        [Fact]
        public async Task CreateTokenByRefreshToken_WhenTokenNotFound_ShouldReturnFailure()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync((AppUserRefreshToken)null!);

            // Act
            var result = await _authService.CreateTokenByRefreshToken("invalidToken");

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(RefreshTokenNotFound, result.ErrorMessage![0]);
        }

        [Fact]
        public async Task CreateTokenByRefreshToken_WhenTokenExpired_ShouldReturnFailure()
        {
            // Arrange
            var expired = new AppUserRefreshToken { ExpiresAt = DateTime.UtcNow.AddMinutes(-10) };

            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync(expired);

            // Act
            var result = await _authService.CreateTokenByRefreshToken("expiredToken");

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(RefreshTokenExpired, result.ErrorMessage![0]);
        }

        [Fact]
        public async Task CreateTokenByRefreshToken_WhenValid_ShouldReturnNewToken()
        {
            // Arrange
            var user = CreateUser();
            var validToken = new AppUserRefreshToken { UserId = user.Id, ExpiresAt = DateTime.UtcNow.AddMinutes(30) };
            var newToken = CreateTokenDTO();

            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync(validToken);

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.CreateToken(user)).Returns(newToken);

            // Act
            var result = await _authService.CreateTokenByRefreshToken("validToken");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(newToken.AccessToken, result.Data!.AccessToken);
        }

        // ---------- RevokeToken Tests ----------

        [Fact]
        public async Task RevokeRefreshToken_WhenTokenNotFound_ShouldReturnFailure()
        {
            // Arrange
            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync((AppUserRefreshToken)null!);

            // Act
            var result = await _authService.RevokeRefreshToken("invalidToken");

            // Assert
            Assert.True(result.IsFail);
            Assert.Contains(RefreshTokenNotFound, result.ErrorMessage![0]);
        }

        [Fact]
        public async Task RevokeRefreshToken_WhenTokenExists_ShouldSucceed()
        {
            // Arrange
            var token = new AppUserRefreshToken { Token = "validToken" };

            _unitOfWorkMock.Setup(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()))
                .ReturnsAsync(token);

            // Act
            var result = await _authService.RevokeRefreshToken("validToken");

            // Assert
            Assert.True(result.IsSuccess);
            _unitOfWorkMock.Verify(x => x.AppUserRefreshToken.GetAsync(It.IsAny<Expression<Func<AppUserRefreshToken, bool>>>()), Times.Once);
        }
    }
}
