using CourseTech.API.Controllers;
using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Services;
using CourseTech.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TestProject2.controllers
{
    public class AuthenticationTest
    {
        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly AuthenticationsController _controller;

        public AuthenticationTest()
        {
            _authServiceMock = new Mock<IAuthenticationService>(MockBehavior.Strict);
            _controller = new AuthenticationsController(_authServiceMock.Object);
        }

        private static LoginDTO CreateLoginDto() =>
            new("user@example.com", "Password123!");

        private static AppUserRefreshTokenDTO CreateRefreshTokenDto() =>
            new(Guid.NewGuid(), "sample-refresh-token", DateTime.UtcNow.AddDays(7));

        private static TokenDTO CreateToken() =>
            new(
                "access-token",
                DateTime.UtcNow.AddMinutes(15),
                "refresh-token",
                DateTime.UtcNow.AddDays(7)
            );

        #region Login

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var loginDto = CreateLoginDto();
            var expected = ServiceResult<TokenDTO>.Success(CreateToken());

            _authServiceMock
                .Setup(s => s.CreateTokenAsync(loginDto))
                .ReturnsAsync(expected);

            var result = await _controller.Login(loginDto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            var loginDto = CreateLoginDto();
            var expected = ServiceResult<TokenDTO>.Fail("Invalid credentials");

            _authServiceMock
                .Setup(s => s.CreateTokenAsync(loginDto))
                .ReturnsAsync(expected);

            var result = await _controller.Login(loginDto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region CreateTokenByRefreshToken

        [Fact]
        public async Task CreateTokenByRefreshToken_ValidToken_ReturnsOk()
        {
            var dto = CreateRefreshTokenDto();
            var expected = ServiceResult<TokenDTO>.Success(CreateToken());

            _authServiceMock
                .Setup(s => s.CreateTokenByRefreshToken(dto.Token))
                .ReturnsAsync(expected);

            var result = await _controller.CreateTokenByRefreshToken(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        [Fact]
        public async Task CreateTokenByRefreshToken_InvalidToken_ReturnsBadRequest()
        {
            var dto = CreateRefreshTokenDto();
            var expected = ServiceResult<TokenDTO>.Fail("Refresh token is invalid or expired");

            _authServiceMock
                .Setup(s => s.CreateTokenByRefreshToken(dto.Token))
                .ReturnsAsync(expected);

            var result = await _controller.CreateTokenByRefreshToken(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion

        #region RevokeRefreshToken

        [Fact]
        public async Task RevokeRefreshToken_ValidToken_ReturnsOk()
        {
            var dto = CreateRefreshTokenDto();
            var expected = ServiceResult.Success();

            _authServiceMock
                .Setup(s => s.RevokeRefreshToken(dto.Token))
                .ReturnsAsync(expected);

            var result = await _controller.RevokeRefreshToken(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        [Fact]
        public async Task RevokeRefreshToken_InvalidToken_ReturnsBadRequest()
        {
            var dto = CreateRefreshTokenDto();
            var expected = ServiceResult.Fail("Token already expired or invalid");

            _authServiceMock
                .Setup(s => s.RevokeRefreshToken(dto.Token))
                .ReturnsAsync(expected);

            var result = await _controller.RevokeRefreshToken(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, objectResult.StatusCode);
            Assert.Equal(expected, objectResult.Value);
        }

        #endregion
    }
}
