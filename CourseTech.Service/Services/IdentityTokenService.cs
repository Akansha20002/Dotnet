using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Models;
using CourseTech.Core.Services;
using CourseTech.Shared.Configuration;
using CourseTech.Shared.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CourseTech.Service.Services
{
    public class IdentityTokenService : IIdentityTokenService
    {
        private readonly TokenOption _tokenOption;
        private readonly UserManager<AppUser> _userManager;

        public IdentityTokenService(IOptions<TokenOption> tokenOption, UserManager<AppUser> userManager)
        {
            _tokenOption = tokenOption.Value;
            _userManager = userManager;
        }

        private IEnumerable<Claim> GenerateUserClaims(AppUser user, List<string> audiences)
        {
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.GivenName,$"{user.FirstName + " " + user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            claims.AddRange(audiences.Select(audience => new Claim(JwtRegisteredClaimNames.Aud, audience)));
            
            var userRoles = _userManager.GetRolesAsync(user).Result;

            foreach (var role in userRoles)
            {
                claims.Add(new Claim("roles", role));
            }

            return claims;
        }

        public TokenDTO CreateToken(AppUser user)
        {
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_tokenOption.RefreshTokenExpiration);
            var securityKey = SigningService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                claims: GenerateUserClaims(user, _tokenOption.Audience),
                expires: accessTokenExpiration,
                signingCredentials: signingCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var tokenDTO = new TokenDTO
            (
                accessToken,
                accessTokenExpiration,
                CreateRefreshToken(),
                refreshTokenExpiration
            );
            return tokenDTO;
        }

        private string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }
    }
}