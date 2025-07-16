using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CourseTech.Core.Services;
using CourseTech.Core.Models;
using CourseTech.Service.Services;
using Microsoft.AspNetCore.Identity;
using CourseTech.Shared.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using CourseTech.Core.DTOs.Authentication;
using CourseTech.Shared;

namespace TestProject2.Services
{
    public class IdentityTokenServiceTests
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly IOptions<TokenOption> _tokenOptions;
        private readonly IdentityTokenService _identityTokenService;

        public IdentityTokenServiceTests()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);

            var tokenOption = new TokenOption
            {
                Audience = new List<string> { "www.courseTech.com" },
                Issuer = "courseTech",
                AccessTokenExpiration = 5,
                RefreshTokenExpiration = 600,
                SecurityKey = "your-super-secret-key-your-super-secret-key"
            };

            _tokenOptions = Options.Create(tokenOption);
            _identityTokenService = new IdentityTokenService(_tokenOptions, _userManagerMock.Object);
        }

        [Fact]
        public void CreateToken_ShouldReturnValidToken()
        {
            // Arrange
            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            var roles = new List<string> { "Student" };
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            // Act
            var result = _identityTokenService.CreateToken(user);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.True(result.AcccesTokenExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public void CreateToken_ShouldIncludeUserClaimsInToken()
        {
            // Arrange
            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            var roles = new List<string> { "Student" };
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            // Act
            var result = _identityTokenService.CreateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.AccessToken);

            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.GivenName && c.Value == $"{user.FirstName} {user.LastName}");
            //Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == roles[0]);
        }

        [Fact]
        public void CreateToken_ShouldGenerateUniqueRefreshToken()
        {
            // Arrange
            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            var roles = new List<string> { "Student" };
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            // Act
            var result1 = _identityTokenService.CreateToken(user);
            var result2 = _identityTokenService.CreateToken(user);

            // Assert
            Assert.NotEqual(result1.RefreshToken, result2.RefreshToken);
        }
    }
}
