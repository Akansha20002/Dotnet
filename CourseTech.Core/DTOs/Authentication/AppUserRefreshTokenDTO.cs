namespace CourseTech.Core.DTOs.Authentication
{
    public record AppUserRefreshTokenDTO(Guid UserId, string Token, DateTime ExpiresAt);
}